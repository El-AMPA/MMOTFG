using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    /// <summary>
    /// Uses an item a specified amount of times.
    /// The user can add 'all' to consume ALL instances of that item.
    /// </summary>
    class cUseItem : ICommand
    {
        public override void SetKeywords()
        {
            key_words = new string[]{
                "/consume",
                "/use",
                "/eat",
                "/drink" 
            };
        }

        internal override void Execute(string command, long chatId, string[] args = null)
        {
            if(args.Length == 1) InventorySystem.ConsumeItem(chatId, args[0], 1, command, args);
            else
            {
                if(args[1] == "all") InventorySystem.ConsumeItem(chatId, args[0], -1, command, args);
                else InventorySystem.ConsumeItem(chatId, args[0], int.Parse(args[1]), command, args);
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
