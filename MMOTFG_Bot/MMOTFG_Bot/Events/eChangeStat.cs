using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.ComponentModel;
using System.Threading.Tasks;
using MMOTFG_Bot.Battle;
using MMOTFG_Bot.Communicator;

namespace MMOTFG_Bot.Events
{
    class eChangeStat : Event
    {      
        public StatChange[] statChanges;

        [JsonConverter(typeof(StringEnumConverter))]
        public StatName statToDepend;

        public float threshold;       

        public string message;

        [DefaultValue(-1)]
        public int activations;

        public int timesActivated;

        public override async Task Execute(string chatId) {
            if (activations > 0 && timesActivated >= activations) return;

            float ratio = (float)user.GetStat(statToDepend) / user.GetMaxStat(statToDepend);

            if (threshold == 0 || ratio <= threshold)
            {
                string msg = "";

                foreach (StatChange sc in statChanges)
                {
                    msg += user.ApplyStatChange(sc);
                }

                await TelegramCommunicator.SendText(chatId, msg + message, true);
                timesActivated++;

                await user.CheckDeath(chatId);
            }
        }
    }
}
