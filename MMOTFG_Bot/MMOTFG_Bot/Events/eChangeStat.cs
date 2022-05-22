using MMOTFG_Bot.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    class eChangeStat : Event
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public StatName statToChange;

        //for additive change
        public float change;

        //for multiplicative change
        public float multiple;

        [JsonConverter(typeof(StringEnumConverter))]
        public StatName statToDepend;

        public float threshold;       

        public bool changeMax;

        public string message;

        [DefaultValue(true)]
        public bool activateOnce;

        public bool hasActivated;

        public override async Task Execute(string chatId) {
            if (activateOnce && hasActivated) return;

            if (threshold == 0 || (user.GetStat(statToDepend) / user.GetMaxStat(statToDepend)) <= threshold)
            {
                if (multiple == 0) user.AddToStat(statToChange, change, changeMax);
                else user.MultiplyStat(statToChange, multiple, changeMax);
                await TelegramCommunicator.SendText(chatId, message, true);
                hasActivated = true;
            }
        }
    }
}
