using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    class Battler
    {
        //estudiar para el futuro
        //public Dictionary<string, float> stats = new Dictionary<string, float>();

        //changes inside battle
        public float[] stats;
        protected float[] maxStats;
        //permanent changes
        protected float[] originalStats;
        public List<Attack> attacks;

        public string name;

        public Behaviour onHit;
        public Behaviour onTurnEnd;
        public Behaviour onKill;

        public bool turnOver;

        public string imageName;
        public string imageCaption;

        public float droppedMoney;
        public string droppedItem;
        [DefaultValue(1)]
        public int droppedItemAmount;
        [DefaultValue(1)]
        public int experienceGiven;

        public bool isAlly;
        public bool isPlayer;

        public int experience;
        [DefaultValue(1)]
        public int level;

        public Battler()
        {
            stats = new float[Stats.statNum];
            originalStats = new float[Stats.statNum];
            maxStats = new float[Stats.statNum];
        }

        //Gets a random attack the enemy has enough MP to use (basic attack should always cost 0 to avoid problems)
        public Attack nextAttack()
        {
            int i = attacks.Count - 1;
            while (attacks[i].mpCost > stats[(int)StatName.MP])
                i--;
            int attack = RNG.Next(0, i + 1);
            return attacks[attack];
        }

        public void OnCreate()
        {
            //attacks are automatically sorted by mpCost
            attacks.Sort((a1, a2) => a1.mpCost.CompareTo(a2.mpCost));
            maxStats = (float[])stats.Clone();
            originalStats = (float[])stats.Clone();
            onHit?.setParent(this);
            onKill?.setParent(this);
            onTurnEnd?.setParent(this);
        }

        public void SetStat(StatName stat, float newValue, bool changeMax = false, bool permanent = false)
        {
            int s = (int)stat;
            //When the max value is changed
            if (changeMax)
            {
                //Proportion is maintained for bounded stats
                if (Stats.isBounded(stat))
                {
                    float currentPercent = stats[(int)stat] / maxStats[(int)stat];
                    stats[(int)stat] = (float)Math.Round(newValue * currentPercent, 2);
                }
                else stats[s] = newValue;
                maxStats[s] = newValue;
            }
            //When the current value is changed
            else
            {
                //Bounded stats are clamped
                if (Stats.isBounded(stat))
                {
                    float max = maxStats[s];
                    stats[s] = (float)Math.Round(Math.Clamp(newValue, 0, max), 2);
                }
                else stats[s] = newValue;
            }
            if (permanent) originalStats[s] = newValue;
        }

        public void AddToStat(StatName stat, float change, bool changeMax = false, bool permanent = false)
        {
            float statn = changeMax ? maxStats[(int)stat] : stats[(int)stat];
            SetStat(stat, statn + change, changeMax, permanent);
        }

        public void MultiplyStat(StatName stat, float mult, bool changeMax = false, bool permanent = false)
        {
            float statn = changeMax ? maxStats[(int)stat] : stats[(int)stat];
            SetStat(stat, statn * mult, changeMax, permanent);
        }

        public void New()
        {
            stats = (float[])originalStats.Clone();
            maxStats = (float[])originalStats.Clone();
        }

        public float GetStat(StatName stat)
        {
            return stats[(int)stat];
        }

        public float GetMaxStat(StatName stat)
        {
            return maxStats[(int)stat];
        }

        public float GetOriginalStat(StatName stat)
        {
            return originalStats[(int)stat];
        }

        //For events such as OnHit, OnKill or OnTurnEnd
        public async Task OnBehaviour(string chatId, Behaviour b) {
            if (b != null)
            {
                //If behaviour has already happened or isn't activated by chance, skip
                if (!b.flag || RNG.Next(0, 100) > b.chance * 100) return;
                if (await b.Execute(chatId))
                {
                    if (b.message != null) await TelegramCommunicator.SendText(chatId, b.message);
                    //Events that happen once are deactivated
                    b.flag = !b.activateOnce;
                }           
            }
        }

        public async Task SkipTurn(string chatId)
        {
            turnOver = true;
            await BattleSystem.SavePlayerBattle(chatId);
            await BattleSystem.NextAttack(chatId);
        }

        public Dictionary<string, object> GetSerializable()
        {
            Dictionary<string, object> battlerInfo = new Dictionary<string, object>();

            Dictionary<string, float> statsTemp = new Dictionary<string, float>();

            int i = 0;
            foreach (float sValue in stats)
            {
                statsTemp.Add(Enum.GetName(typeof(StatName), i), sValue);
                i++;
            }

            battlerInfo.Add(DbConstants.BATTLER_INFO_FIELD_CUR_STATS, statsTemp);

            statsTemp = new Dictionary<string, float>();

            i = 0;
            foreach (float sValue in originalStats)
            {
                statsTemp.Add(Enum.GetName(typeof(StatName), i), sValue);
                i++;
            }

            battlerInfo.Add(DbConstants.BATTLER_INFO_FIELD_OG_STATS, statsTemp);

            statsTemp = new Dictionary<string, float>();

            i = 0;
            foreach (float sValue in maxStats)
            {
                statsTemp.Add(Enum.GetName(typeof(StatName), i), sValue);
                i++;
            }

            battlerInfo.Add(DbConstants.BATTLER_INFO_FIELD_MAX_STATS, statsTemp);

            battlerInfo.Add(DbConstants.BATTLER_FIELD_NAME, name);

            battlerInfo.Add(DbConstants.BATTLER_FIELD_MONEY_DROP, droppedMoney);

            battlerInfo.Add(DbConstants.BATTLER_FIELD_ITEM_DROP, droppedItem);

            battlerInfo.Add(DbConstants.BATTLER_FIELD_ITEM_DROP_AMOUNT, droppedItemAmount);

            battlerInfo.Add(DbConstants.BATTLER_FIELD_IS_ALLY, isAlly);

            battlerInfo.Add(DbConstants.BATTLER_FIELD_IS_PLAYER, isPlayer);

            battlerInfo.Add(DbConstants.BATTLER_FIELD_TURN_OVER, turnOver);

            battlerInfo.Add(DbConstants.BATTLER_FIELD_EXPERIENCE, experience);

            battlerInfo.Add(DbConstants.BATTLER_FIELD_LEVEL, level);

            return battlerInfo;
        }

        public void LoadSerializable(Dictionary<string, object> eInfo)
        {
            Dictionary<string, object> statsDB = (Dictionary<string, object>)eInfo[DbConstants.BATTLER_INFO_FIELD_CUR_STATS];

            foreach (KeyValuePair<string, object> keyValue in statsDB)
            {
                StatName index;
                Enum.TryParse(keyValue.Key, true, out index);

                stats[(int)index] = Convert.ToSingle(keyValue.Value);
            }

            statsDB = (Dictionary<string, object>)eInfo[DbConstants.BATTLER_INFO_FIELD_OG_STATS];

            foreach (KeyValuePair<string, object> keyValue in statsDB)
            {
                StatName index;
                Enum.TryParse(keyValue.Key, true, out index);

                originalStats[(int)index] = Convert.ToSingle(keyValue.Value);
            }

            statsDB = (Dictionary<string, object>)eInfo[DbConstants.BATTLER_INFO_FIELD_MAX_STATS];

            foreach (KeyValuePair<string, object> keyValue in statsDB)
            {
                StatName index;
                Enum.TryParse(keyValue.Key, true, out index);

                maxStats[(int)index] = Convert.ToSingle(keyValue.Value);
            }

            name = eInfo[DbConstants.BATTLER_FIELD_NAME].ToString();

            droppedMoney = Convert.ToSingle(eInfo[DbConstants.BATTLER_FIELD_MONEY_DROP]);
            droppedItem = eInfo[DbConstants.BATTLER_FIELD_ITEM_DROP] as string;

            droppedItemAmount = Convert.ToInt32(eInfo[DbConstants.BATTLER_FIELD_ITEM_DROP_AMOUNT]);

            isAlly = Convert.ToBoolean(eInfo[DbConstants.BATTLER_FIELD_IS_ALLY]);

            turnOver = Convert.ToBoolean(eInfo[DbConstants.BATTLER_FIELD_TURN_OVER]);

            experience = Convert.ToInt32(eInfo[DbConstants.BATTLER_FIELD_EXPERIENCE]);

            level = Convert.ToInt32(eInfo[DbConstants.BATTLER_FIELD_LEVEL]);
        }
    }
}
