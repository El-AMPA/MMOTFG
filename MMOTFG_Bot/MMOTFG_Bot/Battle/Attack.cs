using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    class Attack
    {
        public string name;
        public float power;
        public float mpCost;

        public Attack(string name_, float power_, float mpCost_)
        {
            name = name_;
            power = power_;
            mpCost = mpCost_;
        }
    }
}
