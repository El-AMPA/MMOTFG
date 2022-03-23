using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    class cStatus : ICommand
    {
        public override void setDescription()
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

        internal async override void Execute(string command, long chatId, string[] args = null)
        {
            await BattleSystem.showStatus(chatId, BattleSystem.player);
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
