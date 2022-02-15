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

        public Attack(string n, float p, float m)
        {
            name = n;
            power = p;
            mpCost = m;
        }
    }
}
