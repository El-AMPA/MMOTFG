using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Events
{
    /// <summary>
    /// Starts a battle with a certain enemy
    /// </summary>
    class eStartBattle : Event
    {
        public string Enemy;
        public List<string> Enemies;

        public async override Task Execute(long chatId)
        {
            if (Enemy != null)
                await BattleSystem.StartBattle(chatId, JSONSystem.getEnemy(Enemy));
            else
            {
                List<Enemy> enemies = new List<Enemy>();
                foreach (string s in Enemies) enemies.Add(JSONSystem.getEnemy(s));
                await BattleSystem.StartBattle(chatId, enemies);
            }
        }
    }
}
