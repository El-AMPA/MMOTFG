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
            if (threshold == 0 || (user.getStat(statToDepend) / user.getMaxStat(statToDepend)) <= threshold)
            {
                if (multiple == 0) user.addToStat(statToChange, change, changeMax);
                else user.multiplyStat(statToChange, multiple, changeMax);
                return true;
            }
            else return false;
        }
    }
}
