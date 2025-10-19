using Godot;
using GunGame.assets.scripts;
using GunGame.assets.scripts.misc;
using GunGame.assets.scripts.weapon.ammo;

public partial class Bullet : Area2D
{
    [Export] public float InitialDamage = 10.0f;
	[Export] public float Velocity = 100.0f;
    [Export] public float VelocityFallPerSecond = 1.0f;
    [Export] public float BulletLifetimeInSeconds = 5.0f;

    private Sprite2D _bulletSprite;

    private CollisionShape2D _collisionShape;

    private Vector2 _flightDirection = Vector2.Right;
    private float _currentLifetime = 0.0f;
    private int _shootingPlayerId = 0;

	public override void _Ready()
	{
        if (_bulletSprite == null)
        {
            _bulletSprite = GetNode<Sprite2D>("Sprite2D");
        }
        if (_collisionShape == null)
        {
            _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        }
        this.AreaEntered += CheckIfHit;
        
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

    public void Initialize(BulletPreset preset, int playerId)
    {
        _shootingPlayerId = playerId;
        CollisionLayer = (uint)Constants.CollisionMask.Bullet;
        CollisionMask = (uint)(Constants.CollisionMask.Platform | Constants.CollisionMask.Player);

        _flightDirection = preset.FlightDirection.Normalized();
        Velocity = preset.Velocity;
        VelocityFallPerSecond = preset.VelocityFallPerSecond;
        BulletLifetimeInSeconds = preset.LifetimeInSeconds;

        if (_bulletSprite == null)
        {
            _bulletSprite = GetNode<Sprite2D>("Sprite2D");
        }
        _bulletSprite.Texture = GD.Load<Texture2D>(preset.TexturePath);

        if (_collisionShape == null)
        {
            _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        }
        _collisionShape.Shape = ShapeCreator.GetShapeBasedOnShapeType(preset.SizePreset.ShapeType, preset.SizePreset.ShapeDetails);
        _collisionShape.GlobalRotationDegrees = preset.SizePreset.RotationDegrees;
    }

    public void CheckIfHit(Area2D collision)
    {
        if (collision.GetParent() is PlayerMovementRigidbody rb)
        {
            if (rb.PlayerId == _shootingPlayerId)
            {
                return;
            }
            rb.TakeDamage(InitialDamage, _flightDirection);
        }
        QueueFree();
    }
}
