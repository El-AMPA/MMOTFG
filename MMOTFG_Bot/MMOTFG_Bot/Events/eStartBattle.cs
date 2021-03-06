using System.Collections.Generic;
using System.Threading.Tasks;
using MMOTFG_Bot.Communicator;
using MMOTFG_Bot.Battle;
using MMOTFG_Bot.Loader;
using MMOTFG_Bot.Multiplayer;

namespace MMOTFG_Bot.Events
{
    /// <summary>
    /// Starts a battle with a certain enemy
    /// </summary>
    class eStartBattle : Event
    {
        public string Enemy;
        public List<string> Enemies;

        public async override Task Execute(string chatId)
        {
            List<string> enemies = new List<string>();
            if(Enemy != null) enemies.Add(Enemy);
            if(Enemies != null) enemies.AddRange(Enemies);

            await BattleSystem.StartBattleFromNames(chatId, enemies);          
        }
    }
}
