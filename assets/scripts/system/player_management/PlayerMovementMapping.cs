namespace GunGame.assets.scripts.system.player_management
{
    public struct PlayerMovementMapping
    {
        public int DeviceId {  get; set; } // 0 for keyboard, 1+ for joypad (per claude)
        public string JumpCommand {  get; set; }
        public string MoveLeftCommand {  get; set; }
        public string MoveRightCommand {  get; set; }
        public string PickUpItemCommand {  get; set; }
    }
}
