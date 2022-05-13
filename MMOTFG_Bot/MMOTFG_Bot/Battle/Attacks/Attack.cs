using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

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
        public float mpCost;
        public bool affectsSelf;
        protected Battler user;
        protected Battler target;

        public Attack(string name_, float power_, float mpCost_)
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

        public virtual float GetDamage()
        {
            return user.GetStat(StatName.ATK) * power;
        }

        public virtual async Task OnAttack(string chatId) { }
    }
}
