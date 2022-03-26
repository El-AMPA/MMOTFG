using System;
using System.Collections.Generic;
using System.Text;

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
        public Attack[] attacks;

        public int attackNum;

        public string name;

        public Behaviour onHit;
        public Behaviour onTurnEnd;
        public Behaviour onKill;

        public Battler()
        {
        }

        public void onCreate()
        {
            attackNum = attacks.Length;
            maxStats = (float[])stats.Clone();
            originalStats = (float[])stats.Clone();
            onHit?.setParent(this);
            onKill?.setParent(this);
            onTurnEnd?.setParent(this);
        }

        public void setStat(StatName stat, float newValue, bool changeMax = false, bool permanent = false)
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

        public void addToStat(StatName stat, float change, bool changeMax = false, bool permanent = false)
        {
            float statn = changeMax ? maxStats[(int)stat] : stats[(int)stat];
            setStat(stat, statn + change, changeMax, permanent);
        }

        public void multiplyStat(StatName stat, float mult, bool changeMax = false, bool permanent = false)
        {
            float statn = changeMax ? maxStats[(int)stat] : stats[(int)stat];
            setStat(stat, statn * mult, changeMax, permanent);
        }

        public float getStat(StatName stat)
        {
            return stats[(int)stat];
        }

        public float getMaxStat(StatName stat)
        {
            return maxStats[(int)stat];
        }

        public float getOriginalStat(StatName stat)
        {
            return originalStats[(int)stat];
        }

        //For events such as OnHit, OnKill or OnTurnEnd
        public async void OnBehaviour(long chatId, Behaviour b) {
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
    }
}
