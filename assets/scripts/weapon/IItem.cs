namespace GunGame.assets.scripts.weapon
{
    public interface IItem
    {
        string ItemName { get; }
        string Description { get; }
        int HorizontalDirection { get; set; }
        void FlipItemHorizontally();
    }
}
