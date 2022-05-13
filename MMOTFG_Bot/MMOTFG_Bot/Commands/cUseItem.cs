using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MMOTFG_Bot.Inventory;


namespace MMOTFG_Bot.Commands
{
    /// <summary>
    /// Uses an item a specified amount of times.
    /// The user can add 'all' to consume ALL instances of that item.
    /// </summary>
    class cUseItem : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Uses an item. Depending on the word you provide, different things can happen.
Use: consume / use / eat / drink [item name]";
        }
        public override void SetKeywords()
        {
            key_words = new string[]{
                "consume",
                "use",
                "eat",
                "drink" 
            };
        }

        internal override async Task Execute(string command, string chatId, string[] args = null)
        {
            if(args.Length == 1) await InventorySystem.ConsumeItem(chatId, args[0], 1, command, args);
            else
            {
                if(args[1] == "all") await InventorySystem.ConsumeItem(chatId, args[0], -1, command, args);
                else await InventorySystem.ConsumeItem(chatId, args[0], int.Parse(args[1]), command, args);
            }
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /consume itemName nItemsToUse (Optional)
            if (args.Length < 1 || args.Length > 2) return false;

            if (args.Length == 1) return true;

            if (args[1] == "all") return true;

            if (!int.TryParse(args[1], out int numberToUse)) return false;
            if (numberToUse <= 0) return false;

            return true;
        }
    }
}
