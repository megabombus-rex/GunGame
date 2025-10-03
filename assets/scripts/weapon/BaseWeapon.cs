using Godot;
using GunGame.assets.scripts.weapon;
using GunGame.assets.scripts.weapon.ammo;
using System;

public partial class BaseWeapon : Area2D, IHoldableItem
{
    [Export] public BulletType BulletType = BulletType.Small;
	[Export] public float BulletSpeed = 100.0f;
	[Export] public float FireRatePerSecond = 1.0f;
	[Export] public Vector2 BulletSpawnOffset = new Vector2(0.0f, 0.0f);

    public bool IsHeld { get { return _isHeld; } set { _isHeld = value; } }

    public string ItemName => _weaponName;
	public string Description => _weaponDescription;


    private string _weaponName = string.Empty;
	private string _weaponDescription = string.Empty;
    private bool _isHeld = false;
	private float _firingCooldown = 0.0f;
	private float _firingCooldownMaxVal = 1.0f; // this should not be changed

    private PackedScene _bulletScene = GD.Load<PackedScene>("res://scenes/weapon/ammo/bullet.tscn");
	private Texture2D _bulletTexture;
	private Texture2D _weaponTexture;

	private Sprite2D _weaponSprite;

	// this will be called when the weapon is initialized from the outside (weapon spawner eg.)
	public void Initialize(WeaponStatPreset preset) 
	{
		_weaponName = preset.Name;
		_weaponDescription = preset.Description; 
		BulletType = preset.BulletType;
		BulletSpeed = preset.BulletSpeed;
		FireRatePerSecond = preset.FireRatePerSecond;
        _firingCooldownMaxVal = 1.0f / FireRatePerSecond;

		BulletSpawnOffset = preset.BulletSpawnOffset;

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

        if (Input.IsActionPressed("ShootWeapon") && _firingCooldown >= _firingCooldownMaxVal)
		{
			_firingCooldown = 0.0f;
			SpawnBullet();
		}
	}

	private void SpawnBullet()
	{
		var bullet = BulletTypeResolver.InstantiateBullet(_bulletScene, BulletType);
		bullet.GlobalPosition = GlobalPosition + BulletSpawnOffset;
		bullet.Initialize(Vector2.Right, BulletSpeed, _bulletTexture);
		GetTree().Root.AddChild(bullet);
	}
}
