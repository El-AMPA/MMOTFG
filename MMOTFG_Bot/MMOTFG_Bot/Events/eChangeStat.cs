using MMOTFG_Bot.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    class eChangeStat : Event
    {      
        public StatChange[] statChanges;

        [JsonConverter(typeof(StringEnumConverter))]
        public StatName statToDepend;

        public float threshold;       

        public string message;

        [DefaultValue(true)]
        public bool activateOnce;

        public bool hasActivated;

        public override async Task Execute(string chatId) {
            if (activateOnce && hasActivated) return;

            if (threshold == 0 || (user.GetStat(statToDepend) / user.GetMaxStat(statToDepend)) <= threshold)
            {
                string msg = "";

                foreach (StatChange sc in statChanges)
                {
                    msg += user.ApplyStatChange(sc);
                }

                await TelegramCommunicator.SendText(chatId, msg + message, true);
                hasActivated = true;
            }
        }
    }
}
