using Godot;
using GunGame.assets.scripts.weapon.ammo;

namespace GunGame.assets.scripts.weapon
{
    public struct WeaponStatPreset
    {
        public Vector2 BulletSpawnOffset { get; set; }
        public string TexturePath { get; set; }
        public float FireRatePerSecond { get; set; }
        public BulletType BulletType { get; set; }
        public float BulletSpeed { get; set; }
    }
}
