using Godot;

namespace GunGame.assets.scripts.weapon
{
    public interface IHoldableItem : IItem
    {
        bool IsHeld { get; set; }
        Vector2 GlobalPosition { get; set; }
    }
}
