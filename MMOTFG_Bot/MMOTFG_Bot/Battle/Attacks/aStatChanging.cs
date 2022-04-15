using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    class aStatChanging : Attack
    {
        public aStatChanging(string name_, float power_, float mpCost_) : base(name_, power_, mpCost_) { }

        [JsonConverter(typeof(StringEnumConverter))]
        public StatName statToChange;

        //for multiplicative change
        public float multiple;

        //for additive change
        public float change;

        [DefaultValue(true)]
        public bool changeMax;

        public override async Task OnAttack(string chatId) 
        {
            string stat = statToChange.ToString();
            Battler tgt = affectsSelf ? user : target;
            string message = (multiple == 0) ? $"{tgt.name}'s {stat} was changed by {change}!" : $"{tgt.name}'s {stat} was multiplied by {multiple}!";
            await TelegramCommunicator.SendText(chatId, message);
            if (multiple == 0) tgt.AddToStat(statToChange, change, changeMax);
            else tgt.MultiplyStat(statToChange, multiple, changeMax);
        }
    }
}
