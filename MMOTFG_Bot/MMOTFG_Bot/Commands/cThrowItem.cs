using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    class cThrowItem : ICommand
    {
        public override void Init()
        {
            key_words = new string[] {
                "/throw",
                "/delete",
                "/throw_away"
            };
        }

        internal override void Execute(string command, long chatId, string[] args = null)
        {
            if(args.Length == 1) InventorySystem.ThrowAwayItem(chatId, args[0], 1);
            else
            {
                if (args[1] == "all") InventorySystem.ThrowAwayItem(chatId, args[0], -1);
                else InventorySystem.ThrowAwayItem(chatId, args[0], int.Parse(args[1]));
            }
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /consume itemName nItemsToUse (Optional)
            if (args.Length < 1 || args.Length > 2) return false;

            if (args.Length == 1) return true;

            if (args[1] == "all") return true;

            int numberToUse;
            if (!int.TryParse(args[1], out numberToUse)) return false;
            if (numberToUse <= 0) return false;

            return true;
        }
    }
}
