using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    class Enemy
    {
        public float[] stats; 
        //estudiar para el futuro
        //public Dictionary<string, float> stats = new Dictionary<string, float>();
        public Attack[] attacks;

        public int attackNum;

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
            while (attacks[i].mpCost > stats[(int)StatNames.MP])
                i++;
            int attack = rnd.Next(0, attackNum - i);
            stats[(int)StatNames.MP] -= attacks[attack].mpCost;
            return attacks[attack];
        }

        //para que el enemigo cambie de fase o lo que sea al recibir daño
        virtual public async void OnHit(long chatId) { }

        //para que el enemigo se cure al final del turno y cosas así
        virtual public async void OnTurnEnd(long chatId) { }
    }
}
