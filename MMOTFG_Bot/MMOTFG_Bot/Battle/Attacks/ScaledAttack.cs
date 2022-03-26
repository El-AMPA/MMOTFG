﻿using System;
using System.Collections.Generic;
using System.Text;
using static MMOTFG_Bot.StatName;

namespace MMOTFG_Bot
{
    class ScaledAttack : Attack
    {
        public ScaledAttack(string name_, float power_, float mpCost_) : base(name_, power_, mpCost_) { }

        public override float GetDamage()
        {
            //ataque que se hace más fuerte cuanta menos vida le queda al usuario
            return user.GetStat(ATK) * power * user.GetOriginalStat(HP) / user.GetStat(HP);
        }
    }
}
