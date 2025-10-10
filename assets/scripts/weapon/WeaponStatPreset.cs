using Godot;
using GunGame.assets.scripts.weapon.ammo;

namespace GunGame.assets.scripts.weapon
{
    public struct WeaponStatPreset
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public Vector2 BulletSpawnOffset { get; set; }
        public string TexturePath { get; set; }
        public float FireRatePerSecond { get; set; }
        public BulletType BulletType { get; set; }
        public float BulletSpeed { get; set; }

        public BulletPreset BulletPreset { get; set; }
    }
}
