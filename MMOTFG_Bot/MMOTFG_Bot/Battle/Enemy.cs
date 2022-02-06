using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    class Enemy
    {
        public float[] stats = new float[] { 0, 0 };
        public Attack[] attacks;

        public int attackNum;

        public Enemy()
        {
            stats[(int)StatNames.HP] = 50;
            stats[(int)StatNames.ATK] = 10;

            attacks = new Attack[]{
                new Attack("Arañazo", 1),
                new Attack("Super Arañazo", 2)
            };

            attackNum = attacks.Length;
        }
    }
}
