using Godot;

public partial class PlayerMovement : CharacterBody2D
{
    private enum AnimationState
    {
        standby = 0,
        walk = 1
    }

    private AnimatedSprite2D _animatedSprite;

    //private string _previousAnimation;

    [Export]
    public int Speed { get; set; } = 400;

    [Export]
    public double FrictionX { get; set; } = 0.1;

    private void GetInput()
    {

        Vector2 inputDirection = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        Velocity = inputDirection * Speed;
    }

    public override void _Ready()
    {
        _animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 oldPosition = GlobalPosition;

        GetInput();
        MoveAndSlide();

        Vector2 positionDelta = GlobalPosition - oldPosition;
        if (positionDelta.Length() > 0.01f)
        {
            GD.Print($"[PHYSICS] Moved: {positionDelta}, New Position: {GlobalPosition}");
        }

        if (Velocity.Length() > 0.01f && positionDelta.Length() < 0.01f)
        {
            GD.Print("[PHYSICS] Velocity applied but no movement - possible collision");
        }
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
