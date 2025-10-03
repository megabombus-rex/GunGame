using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GunGame.assets.scripts.misc
{
    public static class RandomExtensions
    {
        public static float NextFloatBetween(this Random rng, float min, float max)
        {
            return (float)((rng.NextDouble() * (max - min)) + min);
        }

        public static double NextDoubleBetween(this Random rng, double min, double max)
        {
            return ((rng.NextDouble() * (max - min)) + min);
        }
    }
}
