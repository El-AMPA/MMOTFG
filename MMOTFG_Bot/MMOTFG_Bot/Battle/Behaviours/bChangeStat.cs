using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    class bChangeStat : Behaviour
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public StatName statToChange;

        public float change;

        public float multiple;

        [JsonConverter(typeof(StringEnumConverter))]
        public StatName statToDepend;

        public float threshold;       

        public bool changeMax;

        public override async Task<bool> Execute(long chatId) {
            if (threshold == 0 || (user.GetStat(statToDepend) / user.GetMaxStat(statToDepend)) <= threshold)
            {
                if (multiple == 0) user.AddToStat(statToChange, change, changeMax);
                else user.MultiplyStat(statToChange, multiple, changeMax);
                return true;
            }
            else return false;
        }
    }
}
