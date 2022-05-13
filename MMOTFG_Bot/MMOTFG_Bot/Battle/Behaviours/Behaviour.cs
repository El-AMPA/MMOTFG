using JsonSubTypes;
using Newtonsoft.Json;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Battle
{
    //If you want to create a new Behaviour class, add it below so JsonSubtypes recognizes it
    //when deserializing the battlers.
    [JsonConverter(typeof(JsonSubtypes), "BehaviourType")]
    [JsonSubtypes.KnownSubType(typeof(bChangeStat), "bChangeStat")]
    [JsonSubtypes.KnownSubType(typeof(bPlayerDeath), "bPlayerDeath")]
    [JsonSubtypes.KnownSubType(typeof(bSetFlag), "bSetFlag")]
    class Behaviour
    {
        [DefaultValue(true)]
        public bool activateOnce;

        [DefaultValue(true)]
        public bool flag;

        [DefaultValue(1)]
        public float chance;

        public string message;

        //returns true on success, false otherwise
        public virtual async Task<bool> Execute(string chatId, Battler user) { return true; }
    }
}
