using Godot;
using GunGame.assets.scripts.misc;
namespace GunGame.assets.scripts.weapon.ammo
{
    public struct BulleColliderDetailsPreset
    {
        public ShapeType ShapeType { get; init; }
        public Vector2 ShapeDetails { get; init; }
        public float RotationDegrees { get; init; }
    }
}
