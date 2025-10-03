using Godot;
using GunGame.assets.scripts.misc;
using System;

namespace GunGame.assets.scripts.system.weapon_spawning.strategies
{
    public class RandomSpawningStrategy : WeaponSpawningStrategy
    {
        private readonly Vector2[] _boundaries = [new Vector2(300.0f, 200.0f), new Vector2(800.0f, 400.0f)];

        public RandomSpawningStrategy(PackedScene weaponScene) : base(weaponScene)
        {
        }

        public override BaseWeapon SpawnWeapon()
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

            return weapon;
        }

    }
}
