using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    /// <summary>
    /// Throws away an item a specified amount of times.
    /// If the user adds "all", it throws away ALL instances of that item.
    /// </summary>
    class cThrowItem : ICommand
    {
        public override void SetKeywords()
        {
            key_words = new string[] {
                "/throw",
                "/delete",
                "/throw_away"
            };
        }

        internal override async void Execute(string command, long chatId, string[] args = null)
        {
            if(args.Length == 1) await InventorySystem.ThrowAwayItem(chatId, args[0], 1);
            else
            {
                if (args[1] == "all") await InventorySystem.ThrowAwayItem(chatId, args[0], -1);
                else await InventorySystem.ThrowAwayItem(chatId, args[0], int.Parse(args[1]));
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
