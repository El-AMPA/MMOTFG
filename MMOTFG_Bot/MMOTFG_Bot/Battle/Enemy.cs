using MMOTFG_Bot.Events;
using MMOTFG_Bot.Navigation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace MMOTFG_Bot
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

        public Enemy()
        {
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

        public new Enemy Clone()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                DefaultValueHandling = DefaultValueHandling.Populate
            };

            string serialize = JsonConvert.SerializeObject(this, settings);

            Enemy e = JsonConvert.DeserializeObject<Enemy>(serialize, settings);

            e.OnClone();
             
            return e;
        }
    }
}
