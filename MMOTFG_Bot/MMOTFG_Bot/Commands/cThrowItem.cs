using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Commands
{
    /// <summary>
    /// Throws away an item a specified amount of times.
    /// If the user adds "all", it throws away ALL instances of that item.
    /// </summary>
    class cThrowItem : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Throws away an item from your inventory. Deleted items can't be recovered.
Use: throw [item name]";
        }
        public override void SetKeywords()
        {
            key_words = new string[] {
                "throw"
            };
        }

        internal override async Task Execute(string command, string chatId, string[] args = null)
        {
            int itemsThrown;

            if(InventorySystem.StringToItem(args[0], out ObtainableItem item))
            {
                if(await InventorySystem.PlayerHasItem(chatId, item))
                {
                    if (args.Length == 1) itemsThrown = await InventorySystem.ThrowAwayItem(chatId, item, 1);
                    else
                    {
                        if (args[1] == "all") itemsThrown = await InventorySystem.ThrowAwayItem(chatId, item, -1);
                        else itemsThrown = await InventorySystem.ThrowAwayItem(chatId, item, int.Parse(args[1]));
                    }
                    if (itemsThrown == 1) await TelegramCommunicator.SendText(chatId, "Item " + args[0] + " was thrown away.");
                    else if (itemsThrown > 1) await TelegramCommunicator.SendText(chatId, "Item " + args[0] + " was thrown away " + itemsThrown + " times");
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
