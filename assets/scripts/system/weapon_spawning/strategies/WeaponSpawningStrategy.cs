using Godot;
using GunGame.assets.scripts.weapon;
using System;
using System.Collections.Generic;

namespace GunGame.assets.scripts.system.weapon_spawning.strategies
{
    public class WeaponSpawningStrategy
    {
        protected PackedScene _weaponScene;

        public WeaponSpawningStrategy(PackedScene weaponScene)
        {
            _weaponScene = weaponScene;
        }

        public virtual BaseWeapon SpawnWeapon()
        {
            throw new NotImplementedException("WeaponSpawningStrategy is an abstract class.");
        }

        protected readonly Dictionary<WeaponType, WeaponStatPreset> _weaponInitializationPresets = new Dictionary<WeaponType, WeaponStatPreset>
        {
            { WeaponType.AK_47,
                new WeaponStatPreset() {
                    Name = "AK-47",
                    Description = "Most famous assault rifle in the world.",
                    BulletSpawnOffset = Vector2.Zero,
                    BulletSpeed = 200.0f,
                    BulletType = GunGame.assets.scripts.weapon.ammo.BulletType.Small,
                    FireRatePerSecond = 1.5f,
                    TexturePath = "res://assets/sprites/weapon/AK-47-16x16.png" }
            },
            { WeaponType.SCAR_H,
                new WeaponStatPreset() {
                    Name = "Scar-H",
                    Description = "This american s...",
                    BulletSpawnOffset = new Vector2(5.0f, 1.0f),
                    BulletSpeed = 200.0f,
                    BulletType = GunGame.assets.scripts.weapon.ammo.BulletType.Small,
                    FireRatePerSecond = 1.5f,
                    TexturePath = "res://assets/sprites/weapon/Scar-H-32x16.png" }
            },
            { WeaponType.M1_GARAND,
                new WeaponStatPreset() {
                    Name = "M1 Garand",
                    Description = "Semi-automatic!",
                    BulletSpawnOffset = new Vector2(10.0f, 2.0f),
                    BulletSpeed = 250.0f,
                    BulletType = GunGame.assets.scripts.weapon.ammo.BulletType.Medium,
                    FireRatePerSecond = 1.0f,
                    TexturePath = "res://assets/sprites/weapon/M1-Garand-16x16.png" }
            },
        };
    }

    public enum StrategyType
    {
        TotallyRandom = 0,
    }

    public static class WeaponSpawningStrategyFactory
    {
        public static WeaponSpawningStrategy CreateStrategy(StrategyType type, PackedScene weaponScene)
        {
            return type switch
            {
                StrategyType.TotallyRandom => new RandomSpawningStrategy(weaponScene),
                _ => throw new NotImplementedException()
            };
        }
    }
}
