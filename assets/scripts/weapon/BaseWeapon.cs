using Godot;
using GunGame.assets.scripts.weapon.ammo;
using System;

public partial class BaseWeapon : Area2D
{
	[Export] public Texture2D BulletTexture;
    [Export] public BulletType BulletType = BulletType.Small;
	[Export] public float BulletSpeed = 100.0f;

	public bool IsHeld { get { return _isHeld; } set { _isHeld = value; } }

	private bool _isHeld = false;
    private PackedScene _bulletScene = GD.Load<PackedScene>("res://scenes/bullet.tscn");
	private string _baseBulletTexturePath = "res://assets/sprites/weapons/ammo/Bullet-S_64x64";

	public override void _Ready()
	{
		if (BulletTexture == null)
		{
			BulletTexture = GD.Load<Texture2D>(_baseBulletTexturePath);
		}
	}

	public override void _Process(double delta)
	{
        if (!_isHeld)
        {
			return;
        }

        if (Input.IsActionJustPressed("ShootWeapon"))
		{
			SpawnBullet();
		}
	}

	private void SpawnBullet()
	{
		var bullet = BulletTypeResolver.InstantiateBullet(_bulletScene, BulletType);
		bullet.GlobalPosition = GlobalPosition; // weapons will have bullet spawn points set manually, based on the texture etc.
		bullet.Initialize(Vector2.Right, BulletSpeed, BulletTexture);
		GetTree().Root.AddChild(bullet);
	}
}
