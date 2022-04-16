using MMOTFG_Bot.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    class Player : Battler
    {
        public LevelUpRoadmap levelUpRoadmap;

        public bool upNext;
        public bool dead;
        [DefaultValue(4)]
        public int maxAttacks;

        public string learningAttack;

        public int experience;
        [DefaultValue(1)]
        public int level;

        public Player()
        {

        }

        public override void OnCreate()
        {
            base.OnCreate();
            stats = (float[])levelUpRoadmap.firstStats.Clone();
            maxStats = (float[])stats.Clone();
            originalStats = (float[])stats.Clone();
            levelUpRoadmap.CalculateLevels();
        }

        private void SetAttackNames()
        {
            attacks = new List<string>();
            foreach (Attack a in attacks_)
            {
                attacks.Add(a.name);
            }
        }
      
        public void SetName(string playerName)
        {
            name = playerName;
        }

        public void SetName(string playerName)
        {
            name = playerName;
        }

        public Attack GetAttack(string name)
        {
            return attacks_.FirstOrDefault(x => x.name.ToLower() == name);
        }

        public async Task GainExperience(long chatId, int exp)
        {
            if (level == levelUpRoadmap.maxLevel) return;
            experience += exp;
            await TelegramCommunicator.SendText(chatId, $"Gained {exp} experience points.");
            //check if new level reached
            int neededExp = levelUpRoadmap.neededExperience[level - 1];
            while (experience >= neededExp)
            {
                level++;
                string statChanges = "";
                for (int i = 0; i < Stats.statNum; i++)
                {
                    int change = (int)levelUpRoadmap.getStatDifference(i);
                    AddToStat((StatName)i, change, changeMax: true, permanent: true);
                    statChanges += $"{(StatName)i} {((change >= 0) ? "+" : "")}{change}\n";
                }
                await TelegramCommunicator.SendText(chatId, $"Reached level {level}!\n" + statChanges);
                //if there is an event for that level
                if (levelUpRoadmap.levelUpEvents != null)
                {
                    var event_ = levelUpRoadmap.levelUpEvents.Find(x => x.level == level);
                    if (event_.ev != null) await event_.ev.Execute(chatId);
                }
                if (level == levelUpRoadmap.maxLevel) return;
                neededExp = levelUpRoadmap.neededExperience[level - 1];
            }
        }

        public async Task LearnAttack(long chatId, string attackName)
        {
            if (attacks_.Count == maxAttacks)
            {
                learningAttack = attackName;
                List<string> options = new List<string>(attacks);
                options.Add("Skip");
                if (BattleSystem.battleActive) await BattleSystem.PauseBattle(chatId);
                await TelegramCommunicator.SendButtons(chatId, $"Do you want to learn {attackName}? Choose an attack to replace or Skip to skip",
                    options, 2, 3);
            }
            else
            {
                learningAttack = null;
                attacks_.Add(JSONSystem.GetAttack(attackName));
                SetAttackNames();
                await TelegramCommunicator.SendText(chatId, $"Learnt {attackName}!");
                if (!BattleSystem.battleActive) await TelegramCommunicator.RemoveReplyMarkup(chatId);
                else if (BattleSystem.battlePaused) await BattleSystem.ResumeBattle(chatId);
            }
            //save move changes
            await BattleSystem.SavePlayerBattle(chatId);
        }

        public async Task ForgetAttack(long chatId, string attackName)
        {
            if (attackName == "skip")
            {
                learningAttack = null;
                await TelegramCommunicator.SendText(chatId, "Skipped move learning");
                if (!BattleSystem.battleActive) await TelegramCommunicator.RemoveReplyMarkup(chatId);
                else if (BattleSystem.battlePaused) await BattleSystem.ResumeBattle(chatId);
                return;
            }
            Attack atk = attacks_.FirstOrDefault(x => x.name.ToLower() == attackName);
            if (atk == null)
            {
                await TelegramCommunicator.SendText(chatId, "Not a valid attack to forget");
            }
            else
            {
                attacks_.Remove(atk);
                await TelegramCommunicator.SendText(chatId, $"Forgot {atk.name}");
                await LearnAttack(chatId, learningAttack);
            }
        }

        public async Task OnBattleOver(long chatId)
        {
            //reset stats to their original value
            for (int i = 0; i < Stats.statNum; i++)
            {
                //(HP/MP only if player died)
                if (!Stats.isBounded((StatName)i) || dead)
                    stats[i] = originalStats[i];
                if (dead)
                {
                    maxStats[i] = originalStats[i];
                }
            }
            //return player to starting node
            if (dead) await Map.SetPlayerPosition(chatId, 0);
            dead = false;
        }

        public override Dictionary<string, object> GetSerializable()
        {
            Dictionary<string, object> playerInfo = base.GetSerializable();

            playerInfo.Add(DbConstants.PLAYER_FIELD_EXPERIENCE, experience);

            playerInfo.Add(DbConstants.PLAYER_FIELD_LEVEL, level);

            SetAttackNames();

            playerInfo.Add(DbConstants.PLAYER_FIELD_ATTACKS, attacks);

            playerInfo.Add(DbConstants.PLAYER_FIELD_UP_NEXT, upNext);

            playerInfo.Add(DbConstants.PLAYER_FIELD_LEARNING_ATTACK, learningAttack);

            return playerInfo;
        }

        public override void LoadSerializable(Dictionary<string, object> eInfo)
        {
            base.LoadSerializable(eInfo);

            experience = Convert.ToInt32(eInfo[DbConstants.PLAYER_FIELD_EXPERIENCE]);

            level = Convert.ToInt32(eInfo[DbConstants.PLAYER_FIELD_LEVEL]);

            List<object> attacksTemp = (List<object>)eInfo[DbConstants.PLAYER_FIELD_ATTACKS];

            attacks = new List<string>();

            foreach (object o in attacksTemp) attacks.Add(Convert.ToString(o));

            SetAttacks();

            upNext = Convert.ToBoolean(eInfo[DbConstants.PLAYER_FIELD_LEVEL]);

            learningAttack = eInfo[DbConstants.PLAYER_FIELD_LEARNING_ATTACK] as string;
        }
    }
}
