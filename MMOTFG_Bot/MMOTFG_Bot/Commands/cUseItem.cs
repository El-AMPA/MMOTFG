using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    class cUseItem : ICommand
    {
        public override void Init()
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
            if(args.Length == 1) InventorySystem.ConsumeItems(chatId, args[0], 1);
            else
            {
                if(args[1] == "all") InventorySystem.ConsumeItems(chatId, args[0], -1);
                else InventorySystem.ConsumeItems(chatId, args[0], int.Parse(args[1]));
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
