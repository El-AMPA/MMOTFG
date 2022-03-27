using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MMOTFG_Bot
{
    class Enemy : Battler
    {
        public string imageName = null;
        public string imageCaption = null;

        public float droppedMoney;
        public string droppedItem;
        [DefaultValue(1)]
        public int droppedItemAmount;

        public Enemy() { }
    
        //Gets a random attack the enemy has enough MP to use (basic attack should always cost 0 to avoid problems)
        public Attack nextAttack()
        {
            int i = attackNum - 1;
            while (attacks[i].mpCost > stats[(int)StatName.MP])
                i--;
            int attack = RNG.Next(0, i+1);
            return attacks[attack];
        }


        public Dictionary<string, object> getSerializable()
        {
            Dictionary<string, object> enemyInfo = new Dictionary<string, object>();

            Dictionary<string, float> statsTemp = new Dictionary<string, float>();

            int i = 0;
            foreach (float sValue in stats)
            {
                statsTemp.Add(Enum.GetName(typeof(StatName), i), sValue);
                i++;
            }

            enemyInfo.Add(DbConstants.BATTLE_INFO_FIELD_CUR_STATS, statsTemp);

            statsTemp = new Dictionary<string, float>();

            i = 0;
            foreach (float sValue in originalStats)
            {
                statsTemp.Add(Enum.GetName(typeof(StatName), i), sValue);
                i++;
            }

            enemyInfo.Add(DbConstants.BATTLE_INFO_FIELD_OG_STATS, statsTemp);

            enemyInfo.Add(DbConstants.ENEMY_FIELD_NAME, name);

            enemyInfo.Add(DbConstants.ENEMY_FIELD_MONEY_DROP, droppedMoney);

            enemyInfo.Add(DbConstants.ENEMY_FIELD_ITEM_DROP, droppedItem);

            enemyInfo.Add(DbConstants.ENEMY_FIELD_ITEM_DROP_AMOUNT, droppedItemAmount);

            return enemyInfo;
        }

        public void loadSerializable(Dictionary<string, object> eInfo)
        {
            Dictionary<string, object> statsDB = (Dictionary<string, object>)eInfo[DbConstants.BATTLE_INFO_FIELD_CUR_STATS];

            foreach (KeyValuePair<string, object> keyValue in statsDB)
            {
                StatName index;
                Enum.TryParse(keyValue.Key, true, out index);

                stats[(int)index] = Convert.ToSingle(keyValue.Value);
            }

            statsDB = (Dictionary<string, object>)eInfo[DbConstants.BATTLE_INFO_FIELD_OG_STATS];

            foreach (KeyValuePair<string, object> keyValue in statsDB)
            {
                StatName index;
                Enum.TryParse(keyValue.Key, true, out index);

                originalStats[(int)index] = Convert.ToSingle(keyValue.Value);
            }

            name = eInfo[DbConstants.ENEMY_FIELD_NAME].ToString();

            droppedMoney = Convert.ToSingle(eInfo[DbConstants.ENEMY_FIELD_MONEY_DROP]);
            droppedItem = eInfo[DbConstants.ENEMY_FIELD_ITEM_DROP].ToString();

            droppedItemAmount = Convert.ToInt32(eInfo[DbConstants.ENEMY_FIELD_ITEM_DROP_AMOUNT]);
        }

    }
}
