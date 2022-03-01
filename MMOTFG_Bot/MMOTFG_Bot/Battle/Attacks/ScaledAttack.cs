using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    class ScaledAttack : Attack
    {
        public ScaledAttack(string name_, float power_, float mpCost_) : base(name_, power_, mpCost_) { }

        public override float getDamage()
        {
            //ataque que se hace más fuerte cuanta menos vida le queda al usuario
            return user.stats[(int)StatName.ATK] * power * user.originalStats[(int)StatName.HP] / user.stats[(int)StatName.HP];
        }
    }
}
