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
        public List<string> attackNames;

        public LevelUpRoadmap levelUpRoadmap;

        public bool upNext;
        public bool dead;
        [DefaultValue(4)]
        public int maxAttacks;

        public Attack learningAttack;

        public Player()
        {

        }

        public void AfterCreate()
        {
            SetAttackNames();
            Program.SetAttackKeywords(attackNames);
            stats = (float[])levelUpRoadmap.firstStats.Clone();
            maxStats = (float[])stats.Clone();
            originalStats = (float[])stats.Clone();
            levelUpRoadmap.CalculateLevels();
        }

        private void SetAttackNames()
        {
            attackNames = new List<string>();
            foreach (Attack a in attacks_)
            {
                attackNames.Add(a.name);
            }
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

        public async Task LearnAttack(long chatId, Attack attack)
        {
            if (attacks_.Count == maxAttacks)
            {
                learningAttack = attack;
                List<string> options = new List<string>(attackNames);
                options.Add("Skip");
                if (BattleSystem.battleActive) await BattleSystem.PauseBattle(chatId);
                await TelegramCommunicator.SendButtons(chatId, $"Do you want to learn {attack.name}? Choose an attack to replace or Skip to skip",
                    options.ToArray(), 2, 3);
                Program.SetAttackKeywords(options);
            }
            else
            {
                learningAttack = null;
                attacks_.Add(attack);
                SetAttackNames();
                Program.SetAttackKeywords(attackNames);
                await TelegramCommunicator.SendText(chatId, $"Learnt {attack.name}!");
                if (!BattleSystem.battleActive) await TelegramCommunicator.RemoveReplyMarkup(chatId);
                else if (BattleSystem.battlePaused) await BattleSystem.ResumeBattle(chatId);
            }
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
            }
            //return player to starting node
            if (dead) await Map.SetPlayerPosition(chatId, 0);
            dead = false;
        }
    }
}
