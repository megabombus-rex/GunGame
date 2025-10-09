using Godot;
using GunGame.assets.scripts.misc;
using GunGame.assets.scripts.weapon;
using GunGame.assets.scripts.weapon.ammo;
using System;

public partial class BaseWeapon : Area2D, IHoldableItem
{
    [Export] public BulletType BulletType = BulletType.Small;
	[Export] public float FireRatePerSecond = 1.0f;
	[Export] public Vector2 BulletSpawnOffset = new Vector2(0.0f, 0.0f);

    public bool IsHeld { get => _isHeld; set { _isHeld = value; } }

    public string ItemName => _weaponName;
	public string Description => _weaponDescription;

    public int HorizontalDirection { get => _direction; set { _direction = value; } }

	private int _direction = 1;
    private string _weaponName = string.Empty;
	private string _weaponDescription = string.Empty;
    private bool _isHeld = false;
	private float _firingCooldown = 0.0f;
	private float _firingCooldownMaxVal = 1.0f; // this should not be changed

    private PackedScene _bulletScene = GD.Load<PackedScene>("res://scenes/weapon/ammo/bullet.tscn");
	private Texture2D _bulletTexture;
	private Texture2D _weaponTexture;

	private Sprite2D _weaponSprite;

	private BulletPreset _bulletPreset;

	// this will be called when the weapon is initialized from the outside (weapon spawner eg.)
	public void Initialize(WeaponStatPreset preset) 
	{
		_weaponName = preset.Name;
		_weaponDescription = preset.Description; 
		BulletType = preset.BulletType;
		FireRatePerSecond = preset.FireRatePerSecond;
        _firingCooldownMaxVal = 1.0f / FireRatePerSecond;

		BulletSpawnOffset = preset.BulletSpawnOffset;

		_bulletPreset = preset.BulletPreset;

        try
		{
			_weaponTexture = GD.Load<Texture2D>(preset.TexturePath);
        }
        catch (Exception e)
		{
			GD.Print("Exception while loading weapon texture: " + e);
            _weaponTexture = GD.Load<Texture2D>("res://assets/sprites/blocks/Blank-texture-64x64.png");
        }
	}

    public override void _Ready()
	{
		_bulletTexture = GD.Load<Texture2D>(BulletTypeResolver.GetBulletTexturePath(BulletType));
		_weaponSprite = GetNode<Sprite2D>("Sprite");
        _weaponSprite.Texture = _weaponTexture;
        _firingCooldownMaxVal = 1.0f / FireRatePerSecond;
	}

	public override void _Process(double delta)
	{
        if (!_isHeld)
        {
			return;
        }

		_firingCooldown += (float)delta;        
	}

    public void UseItem(string actionName, int deviceId)
    {
        if (InputWrapper.IsActionPressed(actionName, deviceId) && _firingCooldown >= _firingCooldownMaxVal)
        {
            _firingCooldown = 0.0f;
            SpawnBullet();
        }
    }

    public void FlipItemHorizontally()
    {
        _direction = -_direction;
        GlobalScale = new Vector2(GlobalScale.X, (GlobalScale.Y * _direction));
		GlobalRotationDegrees = Mathf.Ceil(GlobalRotationDegrees + 180.0f);
    }

    private void SpawnBullet()
	{
		var bullet = BulletTypeResolver.InstantiateBullet(_bulletScene, BulletType);
		bullet.GlobalPosition = GlobalPosition + BulletSpawnOffset;

        // some guns may shoot not only in just a line so the direction will need some work if/when implemented
		// eg. shotguns, changed accuracy etc.
        _bulletPreset.FlightDirection = GetDirectionVector(_direction);

		if (GetParent() is PlayerMovementRigidbody player)
		{
            bullet.Initialize(_bulletPreset, player.PlayerId);
            GetTree().Root.AddChild(bullet);
        }
    }

	private Vector2 GetDirectionVector(int direction)
	{
		return direction switch
		{
			-1 => Vector2.Left,
			1 => Vector2.Right,
			_ => Vector2.Zero
		};
	}
}
