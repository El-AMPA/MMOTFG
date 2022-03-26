using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static MMOTFG_Bot.StatName;

namespace MMOTFG_Bot
{
    class Battler
    {
        //estudiar para el futuro
        //public Dictionary<string, float> stats = new Dictionary<string, float>();
        protected float[] stats;
        protected float[] originalStats;
        public Attack[] attacks;

        public int attackNum;

        public string name;

        public Battler()
        {
        }

        public void SetStat(StatName stat, float newValue)
        {
            //si es un stat que no puede pasarse de cierto límite
            if (stat == HP || stat == MP)
            {
                float max = originalStats[(int)stat];
                stats[(int)stat] = (float)Math.Round(Math.Clamp(newValue, 0, max), 2);
            }
            stats[(int)stat] = newValue;
        }

        public void changeStat(StatName stat, float change)
        {
            //si es un stat que no puede pasarse de cierto límite
            if (stat == HP || stat == MP)
            {
                float current = stats[(int)stat];
                float max = originalStats[(int)stat];
                stats[(int)stat] = (float)Math.Round(Math.Clamp(current + change, 0, max), 2);
            }

            else stats[(int)stat] += change;
        }

        public float GetStat(StatName stat)
        {
            return stats[(int)stat];
        }

        public void SetOriginalStat(StatName stat, float newValue)
        {
            //se mantiene la proporción para el MP y el HP
            if (stat == HP || stat == MP)
            {
                float currentPercent = stats[(int)stat] / originalStats[(int)stat];
                stats[(int)stat] = newValue * currentPercent;
            }
            else stats[(int)stat] = newValue;
            originalStats[(int)stat] = newValue;
        }

        public void ChangeOriginalStat(StatName stat, float change)
        {
            //se mantiene la proporción para el MP y el HP
            if (stat == HP || stat == MP)
            {
                float currentPercent = stats[(int)stat] / originalStats[(int)stat];
                originalStats[(int)stat] += change;
                stats[(int)stat] = originalStats[(int)stat] * currentPercent;
            }

            else
            {
                originalStats[(int)stat] += change;
                stats[(int)stat] = originalStats[(int)stat];
            }
        }

        public float GetOriginalStat(StatName stat)
        {
            return originalStats[(int)stat];
        }

        //para eventos al recibir daño
        virtual public Task OnHit(long chatId) { return Task.CompletedTask; }

        //para eventos al morir
        virtual public Task  OnKill(long chatId) { return Task.CompletedTask; }

        //para eventos de final de turno
        virtual public Task OnTurnEnd(long chatId) { return Task.CompletedTask; }
    }
}
