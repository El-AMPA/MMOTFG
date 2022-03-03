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
            {"Peter", new Peter()}
        };

        public static Enemy getEnemy(string name)
        {
            if (enemyDict.ContainsKey(name))
            {
                return enemyDict[name];
            }
            else return new Enemy();
        }
    }
}
