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
        [EnumMember(Value = "MP")]
        MP
    };

    static class Stats
    {
        public static int statNum = Enum.GetNames(typeof(StatName)).Length;
    }
}
