using System;

namespace GunGame.assets.scripts
{
    public static class Constants
    {
        [Flags]
        public enum CollisionMask
        {
            Player = 1,
            Platform = 2,
            Item = 4,
            Bullet = 8,
        }
    }
}
