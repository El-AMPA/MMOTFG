using MMOTFG_Bot.Events;
using MMOTFG_Bot.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using MMOTFG_Bot.Communicator;
using MMOTFG_Bot.Persistency;
using MMOTFG_Bot.Multiplayer;
using MMOTFG_Bot.Loader;

namespace MMOTFG_Bot.Battle
{
    class Player : Battler
    {
        public LevelUpRoadmap levelUpRoadmap;

        public bool upNext;
        [DefaultValue(4)]
        public int maxAttacks;

        public string learningAttack;

        public int experience;
        [DefaultValue(1)]
        public int level;

        public string id;

        public override void OnCreate()
        {
            base.OnCreate();
            stats = (int[])levelUpRoadmap.firstStats.Clone();
            maxStats = (int[])stats.Clone();
            originalStats = (int[])stats.Clone();
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

        public void SetId(string chatId)
        {
            id = chatId;
        }

        public Attack GetAttack(string name)
        {
            return attacks_.FirstOrDefault(x => x.name.ToLower() == name);
        }

        public async Task GainExperience(string chatId, int exp)
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
                    int change = levelUpRoadmap.getStatDifference(i);
                    AddToStat((StatName)i, change, changeMax: true, permanent: true);
                    statChanges += $"{(StatName)i} {((change >= 0) ? "+" : "")}{change}\n";
                }
                await TelegramCommunicator.SendText(chatId, $"Reached level {level}!\n" + statChanges);
                //if there is an event for that level
                if (levelUpRoadmap.levelUpEvents != null)
                {
                    var lvlup = levelUpRoadmap.levelUpEvents.Find(x => x.level == level);
                    if (lvlup.events != null)
                    {
                        await ProgressKeeper.LoadSerializable(chatId);

                        foreach (Event e in lvlup.events)
                        {
                            e.SetUser(this);
                            await e.ExecuteEvent(chatId);
                        }

                        await ProgressKeeper.SaveSerializable(chatId);
                    }
                }
                if (level == levelUpRoadmap.maxLevel) return;
                neededExp = levelUpRoadmap.neededExperience[level - 1];
            }
        }

        public async Task LearnAttack(string chatId, string attackName)
        {
            if (attacks_.Count == maxAttacks)
            {
                learningAttack = attackName;
                List<string> options = new List<string>(attacks);
                options.Add("Skip");
                if (BattleSystem.battleActive) await BattleSystem.PauseBattle(chatId);
                await TelegramCommunicator.SendButtons(chatId, $"Do you want to learn {attackName}? " +
                    $"Choose an attack to replace or Skip to skip", options);
            }
            else
            {
                learningAttack = null;
                attacks_.Add(JSONSystem.GetAttack(attackName));
                SetAttackNames();
                SetAttacks();
                await TelegramCommunicator.RemoveReplyMarkup(chatId, $"Learnt {attackName}!");
                if (BattleSystem.battlePaused) await BattleSystem.ResumeBattle(chatId);
            }
            //save move changes
            await BattleSystem.SavePlayerBattle(chatId);
        }

        public async Task ForgetAttack(string chatId, string attackName)
        {
            if (attackName == "skip")
            {
                learningAttack = null;
                await TelegramCommunicator.RemoveReplyMarkup(chatId, "Skipped attack learning");
                if (BattleSystem.battlePaused) await BattleSystem.ResumeBattle(chatId);
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

        public async Task OnBattleOver(string chatId)
        {
            //reset stats to their original value
            for (int i = 0; i < Stats.statNum; i++)
            {
                //(HP/MP not restored)
                if (!Stats.isBounded((StatName)i))
                    stats[i] = originalStats[i];
                maxStats[i] = originalStats[i];
            }
            if (stats[(int)StatName.HP] <= 0)
            {
                await OnDeath(chatId);
            }
        }

        public async override Task OnDeath(string chatId)
        {
            bool inParty = await PartySystem.IsInParty(chatId);
            string code = null;
            if (inParty) code = await PartySystem.GetPartyCode(chatId);
            //full wipeout
            if (!inParty || await PartySystem.IsPartyWipedOut(code))
            {
                stats = (int[])originalStats.Clone();
                //return player to starting node
                await TelegramCommunicator.SendText(chatId, "Returning to starting point...");
                await Map.SetPlayerPosition(chatId, 0);
            }
            //in party but not full wipeout
            else
            {
                stats = (int[])originalStats.Clone();
                //1 HP and 1 MP
                stats[(int)StatName.HP] = 1;
                stats[(int)StatName.MP] = 1;
            }
        }

        public override Dictionary<string, object> GetSerializable()
        {
            Dictionary<string, object> playerInfo = base.GetSerializable();

            playerInfo.Add(DbConstants.PLAYER_FIELD_EXPERIENCE, experience);

            playerInfo.Add(DbConstants.PLAYER_FIELD_LEVEL, level);

            SetAttackNames();

            playerInfo.Add(DbConstants.PLAYER_FIELD_ATTACKS, attacks);

            playerInfo.Add(DbConstants.BATTLE_UP_NEXT, upNext);

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

            upNext = Convert.ToBoolean(eInfo[DbConstants.BATTLE_UP_NEXT]);

            learningAttack = eInfo[DbConstants.PLAYER_FIELD_LEARNING_ATTACK] as string;
        }
    }
}
