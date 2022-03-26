using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Commands
{
    class cAddItem : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Adds an item to your inventory. Only for lazy devs :)
Use: add [item name]";
        }

        public override void SetKeywords()
        {
            key_words = new string[] {
                "add"
            };
        }

        internal override async Task Execute(string command, long chatId, string[] args = null)
        {
            if(InventorySystem.StringToItem(args[0], out ObtainableItem item))
            {
                if (args.Length == 1) await InventorySystem.AddItem(chatId, item, 1);
                else await InventorySystem.AddItem(chatId, item, int.Parse(args[1]));
            }
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /consume itemName nItemsToUse (Optional)
            if (args.Length < 1 || args.Length > 2) return false;

            if (args.Length == 1) return true;

            if (!int.TryParse(args[1], out int numberToUse)) return false;
            if (numberToUse <= 0) return false;

            return true;
        }
    }
}
