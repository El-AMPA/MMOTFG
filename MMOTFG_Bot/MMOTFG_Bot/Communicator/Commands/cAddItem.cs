using System.Threading.Tasks;
using MMOTFG_Bot.Inventory;

namespace MMOTFG_Bot.Communicator
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

        internal override async Task Execute(string command, string chatId, string[] args = null)
        {
            if (InventorySystem.StringToItem(args[0], out Item item))
            {
                if (args.Length == 1) await InventorySystem.AddItem(chatId, item, 1);
                else await InventorySystem.AddItem(chatId, item, int.Parse(args[1]));
            }
            else await TelegramCommunicator.SendText(chatId, "Item " + args[0] + " doesn't exist");
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
