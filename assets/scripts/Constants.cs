using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunGame.assets.scripts
{
    public static class Constants
    {
        [Flags]
        public enum CollisionMask
        {
            Player = 1,
            Platform = 2,
            Items = 4
        }
    }
}
