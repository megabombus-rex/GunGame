using GunGame.assets.scripts.misc;
using System.Numerics;

namespace GunGame.assets.scripts.system.player_management
{
    public struct PlayerDisplayAndPhysics
    {
        public string AnimationResoucrePath { get; set; }
        public ShapeType ShapeType { get; set; } // should work ok for circle, rectangle and capsule
        public Vector2 ShapeDetails { get; set; }

        // circle => X = radius, Y = nothing
        // rectangle => X = x_side, Y = y_side
        // capsule => X = radius, Y = height
    }
}
