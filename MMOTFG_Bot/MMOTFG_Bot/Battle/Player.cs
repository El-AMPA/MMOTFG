using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    class Player : Battler
    {
        public List<string> attackNames = new List<string>();
        public List<float> attackmpCosts = new List<float>();

        public Player()
        {
            name = "Player";

            stats = new float[] { 100, 10, 50 };

            originalStats = (float[])stats.Clone();

            attacks = new Attack[]{
                new Attack("Tortazo", 1.5f, 0),
                new Attack("Patada", 2, 1),
                new StatReducingAttack("Burla", 0, 5),
                new Attack("Overkill", 100, 100)
            };

            attackNum = attacks.Length;
            foreach(Attack a in attacks)
            {
                attackNames.Add(a.name);
                attackmpCosts.Add(a.mpCost);
            }
        }

        public Dictionary<string, object> getSerializable()
		{
            Dictionary<string, object> combatInfo = new Dictionary<string,object>();

            Dictionary<string, float> statsTemp = new Dictionary<string, float>();

            int i = 0;
			foreach (float sValue in stats)
			{
                statsTemp.Add(Enum.GetName(typeof(StatName),i), sValue);
                i++;
			}

            combatInfo.Add(DbConstants.BATTLE_INFO_FIELD_CUR_STATS, statsTemp);

            statsTemp = new Dictionary<string, float>();

            i = 0;
            foreach (float sValue in originalStats)
            {
                statsTemp.Add(Enum.GetName(typeof(StatName), i), sValue);
                i++;
            }

            combatInfo.Add(DbConstants.BATTLE_INFO_FIELD_OG_STATS, statsTemp);

            return combatInfo;
		}

        public void loadSerializable(Dictionary<string, object> cInfo)
        {
            Dictionary<string, object> statsDB = (Dictionary<string, object>)cInfo[DbConstants.BATTLE_INFO_FIELD_CUR_STATS];

			foreach (KeyValuePair<string,object> keyValue in statsDB)
			{    
                StatName index;
                Enum.TryParse(keyValue.Key, true, out index);

                stats[(int)index] = Convert.ToSingle(keyValue.Value);
            }

            statsDB = (Dictionary<string, object>)cInfo[DbConstants.BATTLE_INFO_FIELD_OG_STATS];

            foreach (KeyValuePair<string, object> keyValue in statsDB)
            {
                StatName index;
                Enum.TryParse(keyValue.Key, true, out index);

                originalStats[(int)index] = Convert.ToSingle(keyValue.Value);
            }

        }

        public override async void OnKill(long chatId)
        {
            //Recuperas toda la vida y mp
            stats[(int)StatName.HP] = originalStats[(int)StatName.HP];
            stats[(int)StatName.MP] = originalStats[(int)StatName.MP];
            await TelegramCommunicator.SendText(chatId, "You died!");
        }

        public void OnBattleOver()
        {
            //reseteamos los stats (excepto HP/MP) a su estado original
            for (int i = 0; i < Stats.statNum; i++)
            {
                StatName sn = (StatName)i;
                if (sn != StatName.HP && sn != StatName.MP)
                    stats[i] = originalStats[i];
            }
        }
    }
}
