using MMOTFG_Bot.Events;
using MMOTFG_Bot.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    class Battler
    {
        //current stats
        public float[] stats;
        //max stats (por HP and MP)
        protected float[] maxStats;
        //original stats (to revert changes in battle)
        protected float[] originalStats;

        protected List<Attack> attacks_;
        //set in json
        public List<string> attacks;

        public string name;

        public List<Event> onHit;
        public List<Event> onTurnEnd;
        public List<Event> onKill;

        public bool turnOver;

        public string imageName;
        public string imageCaption;

        public string droppedItem;
        [DefaultValue(1)]
        public int droppedItemAmount;
        [DefaultValue(1)]
        public int experienceGiven;     

        public Battler()
        {
            stats = new float[Stats.statNum];
            originalStats = new float[Stats.statNum];
            maxStats = new float[Stats.statNum];
        }

        //Gets a random attack the enemy has enough MP to use (basic attack always costs 0 to avoid problems)
        public Attack NextAttack()
        {
            int i = attacks_.Count - 1;
            while (attacks_[i].mpCost > stats[(int)StatName.MP])
                i--;
            //if no attacks available, return base attack
            if (i < 0) return new Attack("Struggle", 1, 0);
            int attack = RNG.Next(0, i + 1); 
            return attacks_[attack];
        }

        public virtual void OnCreate()
        {
            SetAttacks();           
            maxStats = (float[])stats.Clone();
            originalStats = (float[])stats.Clone();
            //by default, every enemy creates a flag upon death
            if (onKill == null) onKill = new List<Event>();
            onKill.Add(new eSetFlag() { Name = name + "Killed", SetAs = true });
        }

        public void SetAttacks()
        {
            attacks_ = new List<Attack>();
            //get attacks by name
            foreach (string s in attacks) attacks_.Add(JSONSystem.GetAttack(s));
            //attacks are automatically sorted by mpCost
            attacks_.Sort((a1, a2) => a1.mpCost.CompareTo(a2.mpCost));
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

        public string GetStatus()
        {
            string s = $"{name} Status:\n";
            for (int i = 0; i < Stats.statNum; i++)
            {
                StatName sn = (StatName)i;
                s += $"{sn}: {GetStat(sn)}";
                if (Stats.isBounded(sn))
                    s += $"/{GetMaxStat(sn)}";
                s += "\n";
            }
            return s;
        }        

        public string GetStatBar(StatName s)
        {
            int green = (int)(10 * GetStat(s) / GetOriginalStat(s));
            string bar = "";
            for (int i = 0; i < 10; i++)
            {
                if (i <= green) bar += "\U0001F7E9"; //green
                else bar += "\U0001F7E5"; //red
            }
            return bar;
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
        public async Task OnBehaviour(string chatId, List<Event> events) {
            if (events != null)
            {
                await ProgressKeeper.LoadSerializable(chatId);

                foreach (Event e in events)
                {
                    e.SetUser(this);
                    await e.ExecuteEvent(chatId);
                }

                await ProgressKeeper.SaveSerializable(chatId);
            }
        }

        public async Task SkipTurn(string chatId)
        {
            turnOver = true;
            await BattleSystem.SavePlayerBattle(chatId);
            await BattleSystem.NextAttack(chatId);
        }

        public virtual Dictionary<string, object> GetSerializable()
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

            battlerInfo.Add(DbConstants.BATTLER_FIELD_ITEM_DROP, droppedItem);

            battlerInfo.Add(DbConstants.BATTLER_FIELD_ITEM_DROP_AMOUNT, droppedItemAmount);

            battlerInfo.Add(DbConstants.BATTLER_FIELD_TURN_OVER, turnOver);
                    
            return battlerInfo;
        }

        public virtual void LoadSerializable(Dictionary<string, object> eInfo)
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

            droppedItem = eInfo[DbConstants.BATTLER_FIELD_ITEM_DROP] as string;

            droppedItemAmount = Convert.ToInt32(eInfo[DbConstants.BATTLER_FIELD_ITEM_DROP_AMOUNT]);

            turnOver = Convert.ToBoolean(eInfo[DbConstants.BATTLER_FIELD_TURN_OVER]);            
        }
    }
}
