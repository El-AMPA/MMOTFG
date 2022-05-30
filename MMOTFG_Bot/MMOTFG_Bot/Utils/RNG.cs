using System;

namespace MMOTFG_Bot
{
    static class RNG
    {
        static private Random rng = new Random();
        
        static public int Next(int minInclusive, int maxExclusive)
        {
            return rng.Next(minInclusive, maxExclusive);
        }
    }
}
