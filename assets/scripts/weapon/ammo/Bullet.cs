using Godot;
using GunGame.assets.scripts.weapon.ammo;

public partial class Bullet : Area2D
{
    [Export] public float InitialDamage = 10.0f;
	[Export] public float Velocity = 100.0f;
    [Export] public float VelocityFallPerSecond = 1.0f;
    [Export] public float BulletLifetimeInSeconds = 5.0f;

    [Export] public Sprite2D BulletSprite;

    private Vector2 _flightDirection = Vector2.Right;
    private float _currentLifetime = 0.0f;

	public override void _Ready()
	{
        if (BulletSprite != null)
        {
            BulletSprite = GetNode<Sprite2D>("Sprite2D");
        }
    }

	public override void _Process(double delta)
    {
        Position += _flightDirection * Velocity * (float)delta;
        Velocity = Mathf.Max(0.0f, Velocity - (VelocityFallPerSecond * (float)delta));
        _currentLifetime += (float)delta;

        if (_currentLifetime > BulletLifetimeInSeconds)
        {
            QueueFree();
        }
    }

    public void Initialize(Vector2 dir, float speed, Texture2D texture = null)
    {
        _flightDirection = dir.Normalized();
        Velocity = speed;

        if (texture != null)
        {
            GetNode<Sprite2D>("Sprite2D").Texture = texture;
        }
    }

    public void Initialize(BulletPreset preset)
    {
        _flightDirection = preset.FlightDirection.Normalized();
        Velocity = preset.Velocity;
        VelocityFallPerSecond = preset.VelocityFallPerSecond;
        BulletLifetimeInSeconds = preset.LifetimeInSeconds;

        if (BulletSprite == null)
        {
            BulletSprite = GetNode<Sprite2D>("Sprite2D");
        }
        BulletSprite.Texture = GD.Load<Texture2D>(preset.TexturePath);
    }
}
