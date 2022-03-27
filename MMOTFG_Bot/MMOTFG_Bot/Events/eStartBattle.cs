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
        public string Enemy = "";

        public async override Task Execute(long chatId)
        {
            await BattleSystem.startBattle(chatId, JSONSystem.getEnemy(Enemy));
        }
    }
}
