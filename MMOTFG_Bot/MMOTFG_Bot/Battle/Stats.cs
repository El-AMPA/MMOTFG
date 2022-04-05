using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MMOTFG_Bot
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

        public static bool isBounded(StatName stat)
        {
            return boundedStats.Contains(stat);
        }
    }
}
