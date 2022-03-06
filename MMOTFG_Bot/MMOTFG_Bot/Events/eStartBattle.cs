using MMOTFG_Bot.Battle.Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Events
{
    /// <summary>
    /// Starts a battle with a certain enemy
    /// </summary>
    class eStartBattle : Event
    {
        public string Enemy;

        public async override void Execute(long chatId)
        {
            BattleSystem.startBattle(chatId, EnemySystem.getEnemy(Enemy));
        }
    }
}
