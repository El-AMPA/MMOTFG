using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MMOTFG_Bot
{
    public enum EQUIPMENT_SLOT
    {
        [EnumMember(Value = "HEAD")]
        HEAD,
        [EnumMember(Value = "FEET")]
        FEET,
        [EnumMember(Value = "HANDS")]
        HANDS,
        [EnumMember(Value = "WRISTS")]
        WRISTS,
        [EnumMember(Value = "FINGER")]
        FINGER,
        [EnumMember(Value = "TRINKET")]
        TRINKET,
        [EnumMember(Value = "WEAPON")]
        WEAPON,
        [EnumMember(Value = "CHEST")]
        CHEST,
        [EnumMember(Value = "LEGS")]
        LEGS,
        [EnumMember(Value = "WAIST")]
        WAIST,
        [EnumMember(Value = "BACK")]
        BACK,
        [EnumMember(Value = "NECK")]
        NECK
    }
}
