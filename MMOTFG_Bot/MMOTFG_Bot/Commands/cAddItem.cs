using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    class cAddItem : ICommand
    {
        public override void Init()
        {
            key_words = new string[] {
                "/add"
            };
        }

        internal override void Execute(string command, long chatId, string[] args = null)
        {
            if(args.Length == 1) InventorySystem.AddItem(chatId, new Potion(), 1);
            else InventorySystem.AddItem(chatId, new Potion(), int.Parse(args[1]));
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /consume itemName nItemsToUse (Optional)
            if (args.Length < 1 || args.Length > 2) return false;

            if (args.Length == 1) return true;

            int numberToUse;
            if (!int.TryParse(args[1], out numberToUse)) return false;
            if (numberToUse <= 0) return false;

            return true;
        }
    }
}
