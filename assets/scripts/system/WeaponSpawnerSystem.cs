using Godot;
using GunGame.assets.scripts.misc;
using GunGame.assets.scripts.system;
using GunGame.assets.scripts.weapon;
using System;
using System.Collections.Generic;

public partial class WeaponSpawnerSystem : Node2D
{
	private WeaponSpawningStrategy _spawningStrategy;

	[Export] public float _spawnRatePerMinute = 10.0f;
	private float _currentSpawnTime = 0.0f;

	private float _spawnTime = 0.0f;

	private readonly Vector2[] _boundaries = [new Vector2(300.0f, 200.0f), new Vector2(800.0f, 400.0f)];

	private PackedScene _weaponScene = GD.Load<PackedScene>("res://scenes/weapon/weapon.tscn");

    public override void _Ready()
	{
		_spawnTime = 60.0f / _spawnRatePerMinute;
	}

	public override void _Process(double delta)
	{
		_currentSpawnTime += (float)delta;

		if (_currentSpawnTime > _spawnTime) 
		{
			_currentSpawnTime = 0.0f;
			SpawnWeaponRandomly();
		}
	}

	private void SpawnWeaponRandomly()
	{
		var rng = new Random();
		var idx = rng.Next(0, 3);

		GD.Print($"Current index: {idx}");

		var en = (WeaponType)idx;


        var preset = _weaponInitializationPresets[en];

		GD.Print($"Preset: {preset.TexturePath}, enum: {en.ToString()}");

		var weapon = _weaponScene.Instantiate<BaseWeapon>();
		weapon.Initialize(preset);

		float X = rng.NextFloatBetween(_boundaries[0].X, _boundaries[1].X), Y = rng.NextFloatBetween(_boundaries[0].Y, _boundaries[1].Y);
		weapon.GlobalPosition = new Vector2(X, Y);

		GetTree().Root.AddChild(weapon);
	}

	public enum WeaponType
	{
		AK_47 = 0,
		SCAR_H = 1,
		M1_GARAND = 2
	}

	private readonly Dictionary<WeaponType, WeaponStatPreset> _weaponInitializationPresets = new Dictionary<WeaponType, WeaponStatPreset>
	{
		{ WeaponType.AK_47, 
			new WeaponStatPreset() { 
				BulletSpawnOffset = Vector2.Zero, 
				BulletSpeed = 200.0f, 
				BulletType = GunGame.assets.scripts.weapon.ammo.BulletType.Small,
				FireRatePerSecond = 1.5f, 
				TexturePath = "res://assets/sprites/weapon/AK-47-16x16.png" } 
		},
		{ WeaponType.SCAR_H,
            new WeaponStatPreset() {
                BulletSpawnOffset = new Vector2(5.0f, 1.0f),
                BulletSpeed = 200.0f,
                BulletType = GunGame.assets.scripts.weapon.ammo.BulletType.Small,
                FireRatePerSecond = 1.5f,
                TexturePath = "res://assets/sprites/weapon/Scar-H-32x16.png" }
		},
        { WeaponType.M1_GARAND,
            new WeaponStatPreset() {
                BulletSpawnOffset = new Vector2(10.0f, 2.0f),
                BulletSpeed = 250.0f,
                BulletType = GunGame.assets.scripts.weapon.ammo.BulletType.Medium,
                FireRatePerSecond = 1.0f,
                TexturePath = "res://assets/sprites/weapon/M1-Garand-16x16.png" }
        },
    };
}
