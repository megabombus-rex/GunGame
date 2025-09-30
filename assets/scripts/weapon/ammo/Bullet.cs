using Godot;

public partial class Bullet : Area2D
{
	[Export] public float InitialSpeed = 100.0f;
	[Export] public Texture2D BulletTexture;
    [Export] public float BulletLifetimeInSeconds = 5.0f; 

	private Vector2 _flightDirection = Vector2.Right;
    private float _currentLifetime = 0.0f;

	public override void _Ready()
	{
        if (BulletTexture != null)
        {
            GetNode<Sprite2D>("Sprite2D").Texture = BulletTexture;
        }
    }

	public override void _Process(double delta)
    {
        Position += _flightDirection * InitialSpeed * (float)delta;
        _currentLifetime += (float)delta;

        if (_currentLifetime > BulletLifetimeInSeconds)
        {
            QueueFree();
        }
    }

    // to be used when instantinated by a bullet
    public virtual void Initialize(Vector2 dir, float speed, Texture2D texture = null)
    {
        _flightDirection = dir.Normalized();
        InitialSpeed = speed;

        if (texture != null)
        {
            GetNode<Sprite2D>("Sprite2D").Texture = texture;
        }
    }
}
