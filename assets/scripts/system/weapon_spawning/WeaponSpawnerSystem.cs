using Godot;
using GunGame.assets.scripts.system.weapon_spawning.strategies;
using System.Collections.Generic;

public partial class WeaponSpawnerSystem : Node2D
{
	private WeaponSpawningStrategy _spawningStrategy;

	private List<BaseWeapon> _weaponList;

	[Export] public float SpawnRatePerMinute = 10.0f;
	[Export] public int MaxWeaponCount = 4;
	private float _currentSpawnTime = 0.0f;
	private float _spawnTime = 0.0f;

	private PackedScene _weaponScene = GD.Load<PackedScene>("res://scenes/weapon/weapon.tscn");

    public override void _Ready()
	{
		_weaponList = new();
		_spawnTime = 60.0f / SpawnRatePerMinute;

		if (_spawningStrategy == null)
		{
			_spawningStrategy = WeaponSpawningStrategyFactory.CreateStrategy(StrategyType.TotallyRandom, _weaponScene);
		}
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
		if (_weaponList.Count > MaxWeaponCount) return;

		var weapon = _spawningStrategy.SpawnWeapon();

		_weaponList.Add(weapon);
		GetTree().Root.AddChild(weapon);
	}
}

public enum WeaponType
{
    AK_47 = 0,
    SCAR_H = 1,
    M1_GARAND = 2
}
