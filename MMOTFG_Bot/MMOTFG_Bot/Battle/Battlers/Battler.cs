using MMOTFG_Bot.Events;
using MMOTFG_Bot.Navigation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    abstract class Battler
    {
        //current stats
        public int[] stats;
        //max stats (por HP and MP)
        protected int[] maxStats;
        //original stats (to revert changes in battle)
        protected int[] originalStats;

        protected List<Attack> attacks_;
        //set in json
        public List<string> attacks;

        public string name;

        public List<Event> onHit;
        public List<Event> onTurnEnd;
        public List<Event> onKill;

        public bool turnOver;

        public virtual void OnCreate()
        {
            SetAttacks();           
            maxStats = (int[])stats.Clone();
            originalStats = (int[])stats.Clone();
            if (onHit == null) onHit = new List<Event>();
            if (onTurnEnd == null) onTurnEnd = new List<Event>();
            if (onKill == null) onKill = new List<Event>();
        }

        public void SetAttacks()
        {
            attacks_ = new List<Attack>();
            //get attacks by name
            if (attacks == null) return;
            foreach (string s in attacks) attacks_.Add(JSONSystem.GetAttack(s));
            //attacks are automatically sorted by mpCost
            attacks_.Sort((a1, a2) => a1.mpCost.CompareTo(a2.mpCost));
        }

        public void SetStat(StatName stat, int newValue, bool changeMax = false, bool permanent = false)
        {
            int s = (int)stat;
            //When the max value is changed
            if (changeMax)
            {
                //Proportion is maintained for bounded stats
                if (Stats.isBounded(stat))
                {
                    float proportion = stats[(int)stat] / maxStats[(int)stat];
                    stats[(int)stat] = (int)(newValue * proportion);
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
                    int max = maxStats[s];
                    stats[s] = (int)(Math.Clamp(newValue, 0, max));
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
            int green = (int)(10 * GetStat(s) / GetMaxStat(s));
            string bar = "";
            for (int i = 0; i < 10; i++)
            {
                if (i <= green) bar += "\U0001F7E9"; //green
                else bar += "\U0001F7E5"; //red
            }
            return bar;
        }

        //returns a string with the changes
        public string AddToStat(StatName stat, int add, bool changeMax = false, bool permanent = false)
        {
            //if change is permanent, it uses the max value
            if(permanent) changeMax = true;
            int statn = changeMax ? maxStats[(int)stat] : stats[(int)stat];
            SetStat(stat, statn + add, changeMax, permanent);
            string message = $"{name}'s {stat} was changed by {add}!\n";
            return message;
        }

        //returns a string with the changes
        public string MultiplyStat(StatName stat, float mult, bool changeMax = false, bool permanent = false)
        {
            int statn = changeMax ? maxStats[(int)stat] : stats[(int)stat];
            SetStat(stat, (int)(statn * mult), changeMax, permanent);
            string message = $"{name}'s {stat} was multiplied by {mult}!\n";
            return message;
        }

        public int GetStat(StatName stat)
        {
            return stats[(int)stat];
        }

        public int GetMaxStat(StatName stat)
        {
            return maxStats[(int)stat];
        }

        public int GetOriginalStat(StatName stat)
        {
            return originalStats[(int)stat];
        }

        //returns a string with the changes
        public string ApplyStatChange(StatChange sc)
        {
            string message = "";
            if (sc.multiple != 1)
            {
                message += MultiplyStat(sc.statToChange, sc.multiple, sc.changeMax);
            }
            if (sc.add != 0)
            {
                message += AddToStat(sc.statToChange, sc.add, sc.changeMax);
            }
            return message;
        }

        //Gets a random attack the battler has enough MP to use
        public Attack NextAttack()
        {
            int i = attacks_.Count - 1;
            while (i >= 0 && attacks_[i].mpCost > stats[(int)StatName.MP])
                i--;
            //if no attacks available, return null
            if (i < 0) return null;
            int attack = RNG.Next(0, i + 1);
            return attacks_[attack];
        }

        public async Task CheckDeath(string chatId)
        {
            if(GetStat(StatName.HP) <= 0)
                await OnBehaviour(chatId, onKill);

            //check again since onKill can revive
            if (GetStat(StatName.HP) <= 0)
            {
                turnOver = true;
                await TelegramCommunicator.SendText(chatId, $"{name} died!");
                await OnDeath(chatId);
            }
        }

        public virtual async Task OnDeath(string chatId)
        {
        } 

        //For events such as OnHit, OnKill or OnTurnEnd
        public async Task OnBehaviour(string chatId, List<Event> events, Battler target = null) {
            if (events != null)
            {
                await ProgressKeeper.LoadSerializable(chatId);

                foreach (Event e in events)
                {
                    e.SetUser(this);
                    if(target != null) e.SetTarget(target);
                    await e.ExecuteEvent(chatId);
                }

                await ProgressKeeper.SaveSerializable(chatId);
            }
        }

        //multiply all stats by a certain factor
        public void BoostStats(float multiple)
        {
            for(int i = 0; i < Stats.statNum; i++)
            {
                MultiplyStat((StatName)i, multiple, true, true);
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

            battlerInfo.Add(DbConstants.BATTLER_INFO_FIELD_CUR_STATS, SerializeStats(stats));

            battlerInfo.Add(DbConstants.BATTLER_INFO_FIELD_OG_STATS, SerializeStats(originalStats));

            battlerInfo.Add(DbConstants.BATTLER_INFO_FIELD_MAX_STATS, SerializeStats(maxStats));

            battlerInfo.Add(DbConstants.BATTLER_FIELD_NAME, name);

            battlerInfo.Add(DbConstants.BATTLER_FIELD_TURN_OVER, turnOver);

            battlerInfo.Add(DbConstants.BATTLER_FIELD_ON_HIT_FLAGS, SerializeEventFlags(onHit));

            battlerInfo.Add(DbConstants.BATTLER_FIELD_ON_TURN_END_FLAGS, SerializeEventFlags(onTurnEnd));

            battlerInfo.Add(DbConstants.BATTLER_FIELD_ON_KILL_FLAGS, SerializeEventFlags(onKill));

            return battlerInfo;
        }

        public Dictionary<string, int> SerializeStats(int[] stats)
        {
            Dictionary<string, int> ret = new Dictionary<string, int>();
            for (int i = 0; i < Stats.statNum; i++)
            {
                ret.Add(Enum.GetName(typeof(StatName), i), stats[i]);
            }
            return ret;
        }

        public List<int> SerializeEventFlags(List<Event> behaviour)
        {
            List<int> ret = new List<int>();
            foreach (Event e in behaviour.Where(e => e as eChangeStat != null))
            {
                eChangeStat ec = e as eChangeStat;
                ret.Add(ec.timesActivated);
            }
            return ret;
        }

        public virtual void LoadSerializable(Dictionary<string, object> eInfo)
        {
            LoadStats(stats, (Dictionary<string, object>)eInfo[DbConstants.BATTLER_INFO_FIELD_CUR_STATS]);

            LoadStats(originalStats, (Dictionary<string, object>)eInfo[DbConstants.BATTLER_INFO_FIELD_OG_STATS]);

            LoadStats(maxStats, (Dictionary<string, object>)eInfo[DbConstants.BATTLER_INFO_FIELD_MAX_STATS]);

            name = eInfo[DbConstants.BATTLER_FIELD_NAME].ToString();

            turnOver = Convert.ToBoolean(eInfo[DbConstants.BATTLER_FIELD_TURN_OVER]);

            LoadEventFlags(onHit, (List<object>)eInfo[DbConstants.BATTLER_FIELD_ON_HIT_FLAGS]);

            LoadEventFlags(onTurnEnd, (List<object>)eInfo[DbConstants.BATTLER_FIELD_ON_TURN_END_FLAGS]);

            LoadEventFlags(onKill, (List<object>)eInfo[DbConstants.BATTLER_FIELD_ON_KILL_FLAGS]);
        }

        public void LoadStats(int[] stats, Dictionary<string, object> statsDB)
        {
            foreach (KeyValuePair<string, object> keyValue in statsDB)
            {
                StatName index;
                Enum.TryParse(keyValue.Key, true, out index);

                stats[(int)index] = Convert.ToInt32(keyValue.Value);
            }
        }

        public void LoadEventFlags(List<Event> behaviour, List<object> flags)
        {
            int i = 0;
            foreach (Event e in behaviour.Where(e => e as eChangeStat != null))
            {
                eChangeStat ec = e as eChangeStat;
                ec.timesActivated = Convert.ToInt32(flags[i]);
                i++;
            }
        }
    }
}
