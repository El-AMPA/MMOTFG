using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Events
{
    /// <summary>
    /// Gives the player the option to learn a new Attack
    /// </summary>
    class eLearnAttack : Event
    {
        public string attack;

        public async override Task Execute(long chatId)
        {
            await BattleSystem.player.LearnAttack(chatId, JSONSystem.GetAttack(attack));
        }
    }
}
