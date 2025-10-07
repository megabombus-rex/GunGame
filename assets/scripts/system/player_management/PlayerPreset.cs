namespace GunGame.assets.scripts.system.player_management
{
    public struct PlayerPreset
    {
        public PlayerMovementMapping MovementMapping { get; init; }
        public PlayerDisplayAndPhysics DisplayAndPhysics { get; init; }
        public PlayerStats Stats { get; init; }
        public int PlayerNumber { get; init; }
    }
}
