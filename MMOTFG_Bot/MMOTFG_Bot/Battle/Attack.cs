﻿using System;
using System.Collections.Generic;
using System.Text;
using static MMOTFG_Bot.StatName;

namespace MMOTFG_Bot
{
    class Attack
    {
        public string name;
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
            return user.GetStat(ATK) * power;
        }

        public virtual void OnAttack(long chatId) { }
    }
}
