using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    class Player
    {
        public float[] stats = new float[] {0,0};
        public Attack[] attacks;

        public int attackNum;
        public List<string> attackNames;

        public Player()
        {
            stats[(int)StatNames.HP] = 100;
            stats[(int)StatNames.ATK] = 10;

            attacks = new Attack[]{
                new Attack("Tortazo", 1.5f),
                new Attack("Patada", 2),
                new Attack("Cabezazo", 5),
                new Attack("Overkill", 100)
            };

            attackNum = attacks.Length;
            attackNames = new List<string>{"Tortazo", "Patada", "Cabezazo", "Overkill"};
        }
    }
}
