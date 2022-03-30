using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    class Player : Battler
    {
        public List<string> attackNames;
        public List<float> attackmpCosts;

        public LevelUpRoadmap levelUpRoadmap;
        public int experience;
        [DefaultValue(1)]
        public int level;

        public bool upNext;

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
            stats = (float[])levelUpRoadmap.firstStats.Clone();
            maxStats = (float[])stats.Clone();
            originalStats = (float[])stats.Clone();
            levelUpRoadmap.CalculateLevels();
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
            await BattleSystem.NextAttack(chatId);
        }

        public void SetName(string playerName)
		{
            name = playerName;
		}

        public async Task GainExperience(long chatId, int exp)
        {
            if (level == levelUpRoadmap.maxLevel) return;
            experience += exp;
            await TelegramCommunicator.SendText(chatId, $"Gained {exp} experience points.");
            //check if new level reached
            int neededExp = levelUpRoadmap.neededExperience[level-1];
            while (experience >= neededExp)
            {
                level++;
                string statChanges = "";
                for(int i = 0; i < Stats.statNum; i++)
                {
                    int change = (int)levelUpRoadmap.getStatDifference(i);
                    AddToStat((StatName)i, change, changeMax: true, permanent: true);
                    statChanges += $"{(StatName)i} {((change >= 0) ? "+" : "")}{change}\n";
                }
                await TelegramCommunicator.SendText(chatId, $"Reached level {level}!\n" + statChanges);
                //if there is an event for that level
                if(levelUpRoadmap.levelUpEvents != null)
                {
                    var event_ = levelUpRoadmap.levelUpEvents.Find(x => x.level == level);
                    if (event_.ev != null) await event_.ev.Execute(chatId);
                }
                if (level == levelUpRoadmap.maxLevel) return;
                neededExp = levelUpRoadmap.neededExperience[level - 1];
            }
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
