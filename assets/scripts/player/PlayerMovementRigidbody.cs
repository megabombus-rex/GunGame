using Godot;
using GunGame.assets.scripts;
using System.Collections.Generic;
using System.Linq;

public partial class PlayerMovementRigidbody : RigidBody2D
{
    private enum AnimationState
    {
        standby = 0,
        walk = 1
    }

    private AnimatedSprite2D _playerAnimation;
    private CollisionShape2D _playerHitbox;
    private Area2D _pickupDetector;
    private List<Area2D> _pickupList = new();
    private Area2D _closestObject = null;
    private Area2D _heldObject = null;

    private bool _isGrounded = false;

    [Export]
    public int MaxSpeed { get; set; } = 40000;
    [Export]
    public float Acceleration { get; set; } = 1.0f;
    [Export]
    public float JumpForce { get; set; } = -400.0f;


    [Export]
    public float GroundRayLength { get; set; } = 75.0f; // For ground detection

    public override void _Ready()
    {
        _playerAnimation = GetNode<AnimatedSprite2D>("PlayerAnimation");
        _playerHitbox = GetNode<CollisionShape2D>("Hitbox");

        _pickupDetector = GetNode<Area2D>("PickupArea");
        _pickupDetector.CollisionMask = 6;
        _pickupDetector.AreaEntered += OnPickupRangeBodyEntered;
        _pickupDetector.AreaExited += OnPickupRangeBodyExited;
    }

	public override void _Process(double delta)
	{
        if (_pickupList.Any())
        {
            FindClosestObjectForPickup();

            if (Input.IsActionJustPressed("PickupItem"))
            {
                if (_heldObject != null) {
                    _heldObject.Reparent(GetOwner());
                    _heldObject = null;
                }

                if (_closestObject != null)
                {
                    _heldObject = _closestObject;
                    _closestObject = null;
                    _heldObject.Reparent(this);
                    _heldObject.Position = new Vector2(0.0f, 0.0f);
                }
            }
        }

        if ((LinearVelocity.X > 0.1 || LinearVelocity.X < -0.1) && _isGrounded)
        {
            _playerAnimation.Play(AnimationState.walk.ToString());
            return;
        }
        if ((LinearVelocity.Y > 0.1 || LinearVelocity.Y < -0.1) && !_isGrounded)
        {
            _playerAnimation.Stop();
            return;
        }
        _playerAnimation.Play(AnimationState.standby.ToString());
    }

    public override void _PhysicsProcess(double delta)
    {
        CheckGrounded();
        HandleMovement(delta, LinearVelocity);
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

    private void HandleMovement(double delta, Vector2 currentVelocity)
    {
        float horizontalInput = Input.GetAxis("ui_left", "ui_right");

        if (horizontalInput != 0)
        {
            float targetVelocity = horizontalInput * MaxSpeed;
            float velocityDifference = targetVelocity - LinearVelocity.X;

            if (Mathf.Abs(velocityDifference) > 10.0f)
            {
                float force = velocityDifference * Acceleration * (float)delta * Mass;
                ApplyCentralForce(new Vector2(force, 0));
            }
        }

        if (Input.IsActionJustPressed("ui_up") && _isGrounded)
        {
            ApplyCentralImpulse(new Vector2(0, JumpForce * Mass));
        }

        return;
    }

    private void OnPickupRangeBodyEntered(Area2D area)
    {
        _pickupList.Add(area);
    }

    private void OnPickupRangeBodyExited(Area2D area)
    {
        _pickupList.Remove(area);

        if (_closestObject == area)
        {
            _closestObject = null;
        }
    }

    private void FindClosestObjectForPickup()
    {
        var closestDistance = float.MaxValue;
        foreach (var item in _pickupList)
        {
            if (item.GlobalPosition.DistanceTo(GlobalPosition) < closestDistance)
            {
                _closestObject = item;
            }
        }
    }
}
