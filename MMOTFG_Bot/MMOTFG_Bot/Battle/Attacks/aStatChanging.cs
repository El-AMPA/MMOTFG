using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    class aStatChanging : Attack
    {
        public aStatChanging(string name_, float power_, float mpCost_) : base(name_, power_, mpCost_) { }

        [JsonConverter(typeof(StringEnumConverter))]
        public StatName statToChange = StatName.ATK;

        public float multiple = 1;

        public bool affectsSelf = false;

        public override async void OnAttack(long chatId) 
        {
            string stat = statToChange.ToString();
            Battler tgt = affectsSelf ? user : target;
            await TelegramCommunicator.SendText(chatId, $"{tgt.name}'s {stat} was multiplied by {multiple}!");
            tgt.stats[(int)statToChange] *= multiple;
        }
    }
}
