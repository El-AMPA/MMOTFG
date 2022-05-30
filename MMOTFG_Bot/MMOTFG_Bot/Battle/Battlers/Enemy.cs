using MMOTFG_Bot.Events;
using System.ComponentModel;

namespace MMOTFG_Bot.Battle
{
    class Enemy : Battler
    {
        public string imageName;
        public string imageCaption;

        public string droppedItem;
        [DefaultValue(1)]
        public int droppedItemAmount;
        [DefaultValue(1)]
        public int experienceGiven;     

        public override void OnCreate()
        {
            base.OnCreate();
            //by default, every enemy creates a flag upon death
            onKill.Add(new eSetFlag() { Name = name + "Killed", SetAs = true });
        }

        //Gets a random attack the enemy has enough MP to use (basic attack always costs 0 to avoid problems)
        public Attack NextAttack()
        {
            int i = attacks_.Count - 1;
            while (i >= 0 && attacks_[i].mpCost > stats[(int)StatName.MP])
                i--;
            //if no attacks available, return base attack
            if (i < 0) return new Attack("Struggle", 1, 0);
            int attack = RNG.Next(0, i + 1); 
            return attacks_[attack];
        }
    }
}
