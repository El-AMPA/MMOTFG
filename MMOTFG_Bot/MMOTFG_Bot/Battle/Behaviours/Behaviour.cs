using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    //If you want to create a new Behaviour class, add it below so JsonSubtypes recognizes it
    //when deserializing the battlers.
    [JsonConverter(typeof(JsonSubtypes), "BehaviourType")]
    [JsonSubtypes.KnownSubType(typeof(bChangeStat), "bChangeStat")]
    [JsonSubtypes.KnownSubType(typeof(bPlayerDeath), "bPlayerDeath")]
    class Behaviour
    {
        protected Battler user;

        [DefaultValue(true)]
        public bool activateOnce;

        [DefaultValue(true)]
        public bool flag;

        [DefaultValue(1)]
        public float chance;

        public string message;

        public void setParent(Battler b)
        {
            user = b;
        }

        //returns true on success, false otherwise
        public virtual async Task<bool> Execute(long chatId) { return true; }
    }
}
