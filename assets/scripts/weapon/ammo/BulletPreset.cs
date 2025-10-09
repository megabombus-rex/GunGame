using Godot;

namespace GunGame.assets.scripts.weapon.ammo
{
    public struct BulletPreset
    {
        public Vector2 FlightDirection { get; set; }
        public float Velocity { get; set; }
        public float Damage { get; set; }
        public float VelocityFallPerSecond { get; set; }
        public float LifetimeInSeconds { get; set; }
        public string TexturePath { get; set; }
        public BulleColliderDetailsPreset SizePreset { get; set; }
    }
}
