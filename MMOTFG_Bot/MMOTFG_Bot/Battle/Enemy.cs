using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    class Enemy : Battler
    {
        public string imageName = null;
        public string imageCaption = null;

        public float droppedMoney;
        public ObtainableItem droppedItem;
        public int droppedItemAmount;

        public Enemy() { }

        //la idea de esto es que los ataques estén ordenados de menor a mayor MP con el básico costando 0 siempre
        public Attack nextAttack()
        {
            int i = attackNum - 1;
            while (attacks[i].mpCost > stats[(int)StatName.MP])
                i--;
            int attack = RNG.Next(0, i+1);
            return attacks[attack];
        }       
    }
}
