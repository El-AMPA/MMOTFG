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

        //para eventos al recibir daño
        virtual public async void OnHit(long chatId) {
            if (onHit != null) await onHit.Execute(chatId);
        }

        //para eventos al morir
        virtual public async void OnKill(long chatId) {
            if (onKill != null) await onKill.Execute(chatId);
        }

        //para eventos de final de turno
        virtual public async void OnTurnEnd(long chatId) {
            if (onTurnEnd != null) await onTurnEnd.Execute(chatId);
        }
    }
}
