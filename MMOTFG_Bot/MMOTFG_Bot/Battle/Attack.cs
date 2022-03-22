﻿using JsonSubTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MMOTFG_Bot
{
    //If you want to create a new Event class, add it below so JsonSubtypes recognizes it
    //when deserializing the map.
    [JsonConverter(typeof(JsonSubtypes), "AttackType")]
    [JsonSubtypes.KnownSubType(typeof(aScaled), "aScaled")]
    [JsonSubtypes.KnownSubType(typeof(aStatChanging), "aStatChanging")]
    class Attack
    {
        [DefaultValue("Basic Attack")]
        public string name;
        [DefaultValue(1)]
        public float power;
        public float mpCost;
        protected Battler user;
        protected Battler target;

        public Attack(string name_, float power_, float mpCost_)
        {
            name = name_;
            power = power_;
            mpCost = mpCost_;
        }

        public void setUser(Battler user_)
        {
            user = user_;
        }

        public void setTarget(Battler target_)
        {
            target = target_;
        }

        public virtual float getDamage()
        {
            return user.stats[(int)StatName.ATK] * power;
        }

        public virtual async void OnAttack(long chatId) { }
    }
}
