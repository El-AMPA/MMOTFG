using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Battle.Enemies
{
    static class EnemySystem
    {
        static Dictionary<string, Enemy> enemyDict = new Dictionary<string, Enemy>
        {
            {"Manuela", new Manuela()},
            {"Peter", new Peter()},
            {"Fedejimbo", new Fedejimbo()},
            {"Sein", new Sein()},
            {"Stratospeers", new Stratospeers()},
            {"Cleonft", new Cleonft()},
            {"Tinky.exe", new Tinkyexe()},
            {"Thiccboi", new Thiccboi()}
        };

        public static Enemy getEnemy(string name)
        {
            if (enemyDict.ContainsKey(name))
            {
                return enemyDict[name];
            }
            else return new Error();
        }
    }
}
