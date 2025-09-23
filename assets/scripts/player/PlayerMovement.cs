using Godot;

public partial class PlayerMovement : CharacterBody2D
{
    private enum AnimationState
    {
        standby = 0,
        walk = 1
    }

    private AnimatedSprite2D _animatedSprite;
    private CollisionShape2D _collisionShape;

    //private string _previousAnimation;

    [Export]
    public int Speed { get; set; } = 400;

    [Export]
    public float FrictionX { get; set; } = 1000.0f;
    [Export]
    public float FrictionY { get; set; } = 2000.0f;

    private Vector2 ApplyFriction(double delta, Vector2 currentVelocity)
    {
        if (currentVelocity.Length() > 0.01f)
        {
            Vector2 frictionForce = -currentVelocity.Normalized() * new Vector2(FrictionX, FrictionY) * (float)delta;

            if (frictionForce.Length() >= currentVelocity.Length())
            {
                currentVelocity = Vector2.Zero;
            }
            else
            {
                currentVelocity += frictionForce;
            }
        }

        return currentVelocity;
    }

    private Vector2 GetUpdatedVelocity(double delta, Vector2 currentVelocity)
    {
        float verticalInput = Input.GetAxis("ui_up", "ui_down");
        float horizontalInput = Input.GetAxis("ui_left", "ui_right");

        Vector2 updatedVelocity = currentVelocity;

        if (verticalInput != 0)
        {
            updatedVelocity.Y = verticalInput * Speed;
        }

        if (horizontalInput != 0)
        {
            updatedVelocity.X = horizontalInput * Speed;
        }

        updatedVelocity = ApplyFriction(delta, updatedVelocity);


        return updatedVelocity;
    }


    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

    }

    public override void _PhysicsProcess(double delta)
    {
        Velocity = GetUpdatedVelocity(delta, Velocity);
        var collision = MoveAndCollide(new Vector2(Velocity.X * (float)delta, Velocity.Y * (float)delta));
    }

    public override void _Process(double delta)
    {     
        if (this.Velocity.X > 0.1 || this.Velocity.X < -0.1)
        {
            _animatedSprite.Play(AnimationState.walk.ToString());
            return;
        }

        _animatedSprite.Play(AnimationState.standby.ToString());
    }
}
