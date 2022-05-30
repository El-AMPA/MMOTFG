using JsonSubTypes;
using Newtonsoft.Json;
using System.ComponentModel;

namespace MMOTFG_Bot.Battle
{
    //If you want to create a new Attack class, add it below so JsonSubtypes recognizes it
    //when deserializing the battlers.
    [JsonConverter(typeof(JsonSubtypes), "AttackType")]
    [JsonSubtypes.KnownSubType(typeof(aScaled), "aScaled")]
    [JsonSubtypes.KnownSubType(typeof(aStatChanging), "aStatChanging")]
    class Attack
    {
        [DefaultValue("Basic Attack")]
        public string name;
        public float power;
        public int mpCost;
        protected Battler user;
        protected Battler target;

        public bool affectsSelf;

        public Attack(string name_, float power_, int mpCost_)
        {
            name = name_;
            power = power_;
            mpCost = mpCost_;
        }

        public void SetUser(Battler user_)
        {
            user = user_;
        }

        public void SetTarget(Battler target_)
        {
            target = target_;
        }

        public virtual int GetDamage()
        {
            return (int)(user.GetStat(StatName.ATK) * power);
        }

        public virtual string OnAttack() { return ""; }
    }
}
