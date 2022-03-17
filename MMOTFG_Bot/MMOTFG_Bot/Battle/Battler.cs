using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    class Battler
    {
        //estudiar para el futuro
        //public Dictionary<string, float> stats = new Dictionary<string, float>();
        public float[] stats;
        public float[] originalStats;
        public Attack[] attacks;

        public int attackNum;

        public string name;

        public Battler()
        {
        }

        public void onCreate()
        {
            attackNum = attacks.Length;
            originalStats = (float[])stats.Clone();
        }

        public void changeHP(float change)
        {
            float hp = stats[(int)StatName.HP];
            float maxHP = originalStats[(int)StatName.HP];
            stats[(int)StatName.HP] = (float)Math.Round(Math.Clamp(hp + change, 0, maxHP),2);
        }

        public void changeMP(float change)
        {
            float mp = stats[(int)StatName.MP];
            float maxMP = originalStats[(int)StatName.MP];
            stats[(int)StatName.MP] = (float)Math.Round(Math.Clamp(mp + change, 0, maxMP), 2);
        }

        //para eventos al recibir daño
        virtual public async void OnHit(long chatId) { }

        //para eventos al morir
        virtual public async void OnKill(long chatId) { }

        //para eventos de final de turno
        virtual public async void OnTurnEnd(long chatId) { }
    }
}
