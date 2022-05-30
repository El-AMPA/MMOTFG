using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

    public struct StatChange
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public StatName statToChange;

        //for additive change
        public int add;

        [DefaultValue(1)]
        //for multiplicative change
        public float multiple;

        //does it change the max value of bounded stats?
        public bool changeMax;

        [DefaultValue(true)]
        //does it affect user or target?
        public bool affectsSelf;

        public string GetInfo()
        {
            string user = (affectsSelf) ? "User" : "Foe";
            string adds = (add == 0) ? "" : $"{add} ";
            string symbol = (add > 0) ? "+" : "";
            string mults = (multiple == 1) ? "" : $"x{multiple}";

            return $"{user} {statToChange} {symbol}{adds}{mults}";
        }
    }

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
