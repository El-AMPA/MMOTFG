using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    class Player : Battler
    {
        public List<string> attackNames;
        public List<float> attackmpCosts;

        public Player()
        {
            
        }

        public void AfterCreate()
        {
            attackNames = new List<string>();
            attackmpCosts = new List<float>();
            foreach (Attack a in attacks)
            {
                attackNames.Add(a.name);
                attackmpCosts.Add(a.mpCost);
            }
        }

        public Dictionary<string, object> GetSerializable()
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

        public void LoadSerializable(Dictionary<string, object> cInfo)
        {
            Dictionary<string, object> statsDB = (Dictionary<string, object>)cInfo[DbConstants.BATTLE_INFO_FIELD_CUR_STATS];

			foreach (KeyValuePair<string,object> keyValue in statsDB)
			{    
                Enum.TryParse(keyValue.Key, true, out StatName index);

                stats[(int)index] = Convert.ToSingle(keyValue.Value);
            }

            statsDB = (Dictionary<string, object>)cInfo[DbConstants.BATTLE_INFO_FIELD_OG_STATS];

            foreach (KeyValuePair<string, object> keyValue in statsDB)
            {
                Enum.TryParse(keyValue.Key, true, out StatName index);

                originalStats[(int)index] = Convert.ToSingle(keyValue.Value);
            }

        }

        public async Task SkipTurn(long chatId)
        {
            turnOver = true;
            await BattleSystem.nextAttack(chatId);
        }

        public void SetName(string playerName)
		{
            name = playerName;
		}

        public void OnBattleOver()
        {
            //reseteamos los stats (excepto HP/MP) a su estado original
            for (int i = 0; i < Stats.statNum; i++)
            {
                if (!Stats.isBounded((StatName)i))
                    stats[i] = originalStats[i];
            }
        }
    }
}
