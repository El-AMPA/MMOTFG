using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    public enum StatNames { HP, ATK, MP };

    static class Stats
    {
        public static int statNum = Enum.GetNames(typeof(StatNames)).Length;
    }
}
