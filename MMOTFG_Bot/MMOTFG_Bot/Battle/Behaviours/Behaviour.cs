using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    [JsonConverter(typeof(JsonSubtypes), "BehaviourType")]
    [JsonSubtypes.KnownSubType(typeof(bChangeStat), "bChangeStat")]
    class Behaviour
    {
        protected Battler user;

        [DefaultValue(true)]
        public bool activateOnce;

        protected bool flag = true;

        public void setParent(Battler b)
        {
            user = b;
        }

        public virtual async Task Execute(long chatId) { }
    }
}
