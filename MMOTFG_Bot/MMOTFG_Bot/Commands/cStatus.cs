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

        internal override async Task Execute(string command, string chatId, string[] args = null)
        {
            if (args.Length == 0)
            {
                await BattleSystem.ShowStatus(chatId);
                await InventorySystem.ShowGear(chatId);
            }
            else
            {
                //If it's someone from your party the game shows their gear too
                string friendId = await PartySystem.GetFriendId(chatId, args[0]);
                await BattleSystem.ShowStatus(chatId, args[0]);
                if (friendId != null) await InventorySystem.ShowGear(chatId, friendId);
            }
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /status enemy (optional)
            if (args.Length > 1) return false;

            return true;
        }
    }
}
