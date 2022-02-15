using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    class Player
    {
        public float[] stats = new float[] {0,0,0};
        public Attack[] attacks;

        public int attackNum;
        public List<string> attackNames = new List<string>();
        public List<float> attackmpCosts = new List<float>();

        public Player()
        {
            stats[(int)StatNames.HP] = 100;
            stats[(int)StatNames.ATK] = 10;
            stats[(int)StatNames.MP] = 50;

            attacks = new Attack[]{
                new Attack("Tortazo", 1.5f, 0),
                new Attack("Patada", 2, 1),
                new Attack("Cabezazo", 5, 5),
                new Attack("Overkill", 100, 100)
            };

            attackNum = attacks.Length;
            foreach(Attack a in attacks)
            {
                attackNames.Add(a.name);
                attackmpCosts.Add(a.mpCost);
            }
        }
    }
}
