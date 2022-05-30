using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using JsonSubTypes;
using System.Threading.Tasks;
using System.ComponentModel;
using MMOTFG_Bot.Navigation;

namespace MMOTFG_Bot.Events
{
    //If you want to create a new Event class, add it below so JsonSubtypes recognizes it
    //when deserializing the map.
    [JsonConverter(typeof(JsonSubtypes), "EventType")]
    [JsonSubtypes.KnownSubType(typeof(eGiveItem), "eGiveItem")]
    [JsonSubtypes.KnownSubType(typeof(eSendAudio), "eSendAudio")]
    [JsonSubtypes.KnownSubType(typeof(eSendImage), "eSendImage")]
    [JsonSubtypes.KnownSubType(typeof(eSendText), "eSendText")]
    [JsonSubtypes.KnownSubType(typeof(eSendImageCollection), "eSendImageCollection")]
    [JsonSubtypes.KnownSubType(typeof(eSetFlag), "eSetFlag")]
    [JsonSubtypes.KnownSubType(typeof(eStartBattle), "eStartBattle")]
    [JsonSubtypes.KnownSubType(typeof(eLearnAttack), "eLearnAttack")]
    [JsonSubtypes.KnownSubType(typeof(eChangeStat), "eChangeStat")]

    //Even though it's used as if it were an abstract class, it can't be abstract because in the process of deserializing into the child class,
    //it creates an instance of the parent class.
    //Thus, if it were a pure abstract class, it would just send an exception.
    class Event
    {
        public string TriggerCondition
        {
            get;
            set;
        }

        protected Battler user;
        protected Battler target;

        [DefaultValue(1)]
        public float Chance = 1;

        public virtual string GetInformation()
        {
            return "Unique effect";
        }

        public virtual Task Execute(string chatId)
        {
            return Task.CompletedTask;
        }

        public async Task<bool> ExecuteEvent(string chatId)
        {
            bool condition = true;

            if (TriggerCondition != null)
            {
                condition = ProgressKeeper.IsFlagActive(chatId, TriggerCondition);
            }

            if (condition)
            {
                if (Chance == 1 || RNG.Next(0, 100) < Chance * 100)
                {
                    await Execute(chatId);
                    return true;
                }
            }
            return false;
        }

        public void SetUser(Battler u)
        {
            user = u;
        }

        public void SetTarget(Battler t)
        {
            target = t;
        }
    }
}
