using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    class Enemy
    {
        public float[] stats = new float[] { 0, 0, 0};
        public Attack[] attacks;

        public int attackNum;

        public string imageName = "manuela.jpg";
        public string imageCaption = "Manuela ataca!";

        public float droppedMoney = 100;
        public ObtainableItem droppedItem = new Potion();
        public int droppedItemAmount = 2;

        public Enemy()
        {
            stats[(int)StatNames.HP] = 50;
            stats[(int)StatNames.ATK] = 10;
            stats[(int)StatNames.MP] = 25;

            attacks = new Attack[]{
                new Attack("Arañazo", 1, 0),
                new Attack("Super Arañazo", 2, 1)
            };

            attackNum = attacks.Length;
        }

        //la idea de esto es que los ataques estén ordenados de menor a mayor MP con el básico costando 0 siempre
        public Attack nextAttack(Random rnd)
        {
            int i = 0;
            while (attacks[i].mpCost > stats[(int)StatNames.MP])
                i++;
            int attack = rnd.Next(0, attackNum - i);
            stats[(int)StatNames.MP] -= attacks[attack].mpCost;
            return attacks[attack];
        }
    }
}
