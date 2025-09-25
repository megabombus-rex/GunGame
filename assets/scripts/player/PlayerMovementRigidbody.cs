using Godot;
using GunGame.assets.scripts;
using System;

public partial class PlayerMovementRigidbody : RigidBody2D
{
    private enum AnimationState
    {
        standby = 0,
        walk = 1
    }

    private AnimatedSprite2D _animatedSprite;
    private CollisionShape2D _collisionShape;

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
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
    }

	public override void _Process(double delta)
	{
        if ((LinearVelocity.X > 0.1 || LinearVelocity.X < -0.1) && _isGrounded)
        {
            _animatedSprite.Play(AnimationState.walk.ToString());
            return;
        }
        if ((LinearVelocity.Y > 0.1 || LinearVelocity.Y < -0.1) && !_isGrounded)
        {
            _animatedSprite.Stop();
            return;
        }
        _animatedSprite.Play(AnimationState.standby.ToString());
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

}
