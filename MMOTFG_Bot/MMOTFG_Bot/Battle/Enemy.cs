using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    class Enemy : Battler
    {
        public string imageName;
        public string imageCaption;

        public float droppedMoney;
        public ObtainableItem droppedItem;
        public int droppedItemAmount;

        public Enemy() { }

        //la idea de esto es que los ataques estén ordenados de menor a mayor MP con el básico costando 0 siempre
        public Attack nextAttack(Random rnd)
        {
            int i = 0;
            while (attacks[i].mpCost > stats[(int)StatName.MP])
                i++;
            int attack = rnd.Next(0, attackNum - i);
            stats[(int)StatName.MP] -= attacks[attack].mpCost;
            return attacks[attack];
        }       
    }
}
