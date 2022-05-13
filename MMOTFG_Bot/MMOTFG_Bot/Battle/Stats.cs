using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MMOTFG_Bot.Battle
{   
    public enum StatName {
        [EnumMember(Value = "HP")]
        HP,
        [EnumMember(Value = "ATK")]
        ATK,
        [EnumMember(Value = "SPE")]
        SPE,
        [EnumMember(Value = "MP")]
        MP
    };

    static class Stats
    {
        private static List<StatName> boundedStats = new List<StatName> { StatName.HP, StatName.MP };

        public static int statNum = Enum.GetNames(typeof(StatName)).Length;

        public static bool IsBounded(StatName stat)
        {
            return boundedStats.Contains(stat);
        }
    }
}
