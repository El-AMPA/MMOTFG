using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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

        internal override async Task Execute(string command, long chatId, string[] args = null)
        {
            int itemsConsumed;

            if (InventorySystem.StringToItem(args[0], out ObtainableItem item))
            {
                if (await InventorySystem.PlayerHasItem(chatId, item))
                {
                    if (args.Length == 1) itemsConsumed = await InventorySystem.ConsumeItem(chatId, item, 1, command, args);
                    else
                    {
                        if (args[1] == "all") itemsConsumed = await InventorySystem.ConsumeItem(chatId, item, -1, command, args);
                        else itemsConsumed = await InventorySystem.ConsumeItem(chatId, item, int.Parse(args[1]), command, args);
                    }
                    if (itemsConsumed == 1) await TelegramCommunicator.SendText(chatId, "Item " + item.name + " was removed from your inventory.");
                    else if (itemsConsumed > 1) await TelegramCommunicator.SendText(chatId, "Item " + item.name + " was removed from your inventory " + args[1] + "times");
                }
                else await TelegramCommunicator.SendText(chatId, "Item " + item.name + " couldn't be found in your inventory");
            }
            else await TelegramCommunicator.SendText(chatId, "Item " + args[1] + " doesn't exist");
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
