using Godot;
using GunGame.assets.scripts;
using GunGame.assets.scripts.misc;
using GunGame.assets.scripts.system.player_management;
using GunGame.assets.scripts.weapon;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerMovementRigidbody : RigidBody2D
{
    [Signal]
    public delegate void HealthChangedEventHandler(int playerId, int oldValue, int newValue);

    private enum AnimationState
    {
        standby = 0,
        walk = 1
    }

    private int _direction = 1;
    private AnimatedSprite2D _playerAnimation;
    private CollisionShape2D _bodyCollision;
    private Area2D _hitbox;
    private Area2D _pickupDetector;
    private List<IHoldableItem> _pickupList = new();
    private IHoldableItem _closestPickableItem = null;
    private IHoldableItem _heldObject = null;

    public int PlayerId { get { return _number; } }

    private int _number;
    private int _deviceId;
    private string _jumpCommand = "p1_jump";
    private string _moveLeftCommand = "p1_left";
    private string _moveRightCommand = "p1_right";
    private string _pickUpCommand = "p1_pickup";
    private string _useItemCommand = "p1_useItem";

    private bool _isGrounded = false;

    [Export] public int MaxSpeed { get; set; } = 40000;
    [Export] public float Acceleration { get; set; } = 1.0f;
    [Export] public float JumpForce { get; set; } = -400.0f;

    [Export] public float GroundRayLength { get; set; } = 75.0f; // For ground detection

    // make the health as '%' as in Smash
    private float _hitpoints = 0.0f;

    private const float HIT_FORCE_MULTIPLIER = 5.0f;

    public override void _Ready()
    {
        GD.Print("Player ready.");

        _playerAnimation = GetNode<AnimatedSprite2D>("PlayerAnimation");
        _bodyCollision = GetNode<CollisionShape2D>("BodyCollision");
        _hitbox = GetNode<Area2D>("Hitbox");

        _pickupDetector = GetNode<Area2D>("PickupArea");
        _pickupDetector.CollisionMask = 6;
    }

    public void Initialize(PlayerPreset preset)
    {
        _jumpCommand = preset.MovementMapping.JumpCommand;
        _moveLeftCommand = preset.MovementMapping.MoveLeftCommand;
        _moveRightCommand = preset.MovementMapping.MoveRightCommand;
        _pickUpCommand = preset.MovementMapping.PickUpItemCommand;
        _useItemCommand = preset.MovementMapping.UseItemCommand;

        MaxSpeed = preset.Stats.MaxSpeed;
        Acceleration = preset.Stats.Acceleration;
        JumpForce = preset.Stats.JumpForce;
        Mass = preset.Stats.Mass;

        _number = preset.PlayerNumber;

        if (_playerAnimation == null)
        {
            _playerAnimation = GetNode<AnimatedSprite2D>("PlayerAnimation");
        }
        _playerAnimation.SpriteFrames = GD.Load<SpriteFrames>(preset.DisplayAndPhysics.AnimationResoucrePath);

        if (_bodyCollision == null)
        {
            _bodyCollision = GetNode<CollisionShape2D>("BodyCollision");
        }
        _bodyCollision.Shape = ShapeCreator.GetShapeBasedOnShapeType(preset.DisplayAndPhysics.ShapeType, preset.DisplayAndPhysics.ShapeDetails);

        if (_hitbox == null)
        {
            _hitbox = GetNode<Area2D>("Hitbox");
        }
        _hitbox.GetNode<CollisionShape2D>("HitboxShape").Shape = ShapeCreator.GetShapeBasedOnShapeType(preset.DisplayAndPhysics.ShapeType, preset.DisplayAndPhysics.ShapeDetails);
        

        GD.Print($"Instantinated player: {_number}, with texture: {_playerAnimation.SpriteFrames.ResourcePath}, with shape: {_bodyCollision.Shape}");
    }

	public override void _Process(double delta)
	{
        PickupProcess();
        ShootingProcess();
        AnimationProcess();
    }

    private void ShootingProcess()
    {
        if (_heldObject == null) return;

        _heldObject.UseItem(_useItemCommand, _deviceId);
    }

    public override void _PhysicsProcess(double delta)
    {
        CheckGrounded();
        HandleMovement(delta);
    }

    public void TakeDamage(float damage, Vector2 direction)
    {
        var hitpointsWithDamage = _hitpoints + damage;
        GD.Print($"Hitpoints before hit: {_hitpoints}, after hit: {hitpointsWithDamage}");
        EmitSignal("HealthChanged", _number, _hitpoints, hitpointsWithDamage);
        _hitpoints = hitpointsWithDamage;
        var force = direction.Normalized() * _hitpoints * HIT_FORCE_MULTIPLIER;
        ApplyCentralImpulse(force);
    }

    private void CheckGrounded()
    {
        var spaceState = GetWorld2D().DirectSpaceState;

        var query = PhysicsRayQueryParameters2D.Create(GlobalPosition, GlobalPosition + Vector2.Down * GroundRayLength);
        query.CollisionMask = (uint)(Constants.CollisionMask.Platform | Constants.CollisionMask.Player);
        query.Exclude = new Godot.Collections.Array<Rid> { GetRid() };

        var result = spaceState.IntersectRay(query);

        _isGrounded = result.Count > 0;
    }

    private void HandleMovement(double delta)
    {
        int horizontalInput = (int)Mathf.Ceil(Input.GetAxis(_moveLeftCommand, _moveRightCommand));

        if (horizontalInput != 0)
        {
            if (_direction != horizontalInput)
            {
                _direction = horizontalInput;
                _playerAnimation.FlipH = !_playerAnimation.FlipH;

                if (_heldObject != null)
                {
                    _heldObject.FlipItemHorizontally();
                }
            }

            float targetVelocity = horizontalInput * MaxSpeed;
            float velocityDifference = targetVelocity - LinearVelocity.X;

            if (Mathf.Abs(velocityDifference) > 10.0f)
            {
                float force = velocityDifference * Acceleration * (float)delta * Mass;
                ApplyCentralForce(new Vector2(force, 0));
            }
        }

        if (Input.IsActionJustPressed(_jumpCommand) && _isGrounded)
        {
            ApplyCentralImpulse(new Vector2(0, JumpForce * Mass));
        }
    }

    private void PickupProcess()
    {
        FindItemsForPickup();

        if (_pickupList.Any())
        {
            FindClosestObjectForPickup();

            if (Input.IsActionJustPressed(_pickUpCommand))
            {
                if (_heldObject != null)
                {
                    LeaveItem();
                }

                if (_closestPickableItem != null)
                {
                    _heldObject = _closestPickableItem;
                    _heldObject.IsHeld = true;

                    if (_closestPickableItem.HorizontalDirection != _direction)
                    {
                        _closestPickableItem.FlipItemHorizontally();
                    }

                    _closestPickableItem = null;

                    if (_heldObject is Node2D node)
                    {
                        node.Reparent(this);
                        node.Position = new Vector2(0.0f, 0.0f);
                    }
                }
            }
            return;
        }

        // holding, but nothing is pickable in range
        if (_heldObject != null)
        {
            if (Input.IsActionJustPressed(_pickUpCommand))
            {
                LeaveItem();
            }
        }
    }

    private void LeaveItem()
    {
        _heldObject.IsHeld = false;

        if (_heldObject is Node2D node)
        {
            node.Reparent(GetTree().Root);
            _heldObject = null;
        }
    }

    private void FindItemsForPickup()
    {
        _pickupList = _pickupDetector.GetOverlappingAreas()
            .Select(a => a as IHoldableItem)
            .Where(a => !a.IsHeld)
            .ToList();
    }

    private void FindClosestObjectForPickup()
    {
        var closestDistance = float.MaxValue;
        foreach (var item in _pickupList)
        {
            if (item.GlobalPosition.DistanceTo(GlobalPosition) < closestDistance)
            {
                _closestPickableItem = item;
            }
        }
    }

    private void AnimationProcess()
    {
        if (_isGrounded)
        {
            if (Mathf.Abs(LinearVelocity.X) > 0.1) 
            {
                _playerAnimation.Play(AnimationState.walk.ToString());
                return;
            }
        }
        else
        {
            if (Mathf.Abs(LinearVelocity.Y) > 0.1)
            {
                _playerAnimation.Stop();
                return;
            }
        }
        _playerAnimation.Play(AnimationState.standby.ToString());
    }
}
