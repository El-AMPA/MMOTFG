using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Commands
{
    class cStatus : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Lists the player's stats.
use: stats";
        }
        public override void SetKeywords()
        {
            key_words = new string[] {
                "status",
                "s"
            };
        }

        internal override async Task Execute(string command, long chatId, string[] args = null)
        {
            await BattleSystem.ShowStatus(chatId, BattleSystem.player);
            await InventorySystem.ShowGear(chatId);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /status enemy (optional)
            if (args.Length > 1) return false;

            return true;
        }
    }
}
