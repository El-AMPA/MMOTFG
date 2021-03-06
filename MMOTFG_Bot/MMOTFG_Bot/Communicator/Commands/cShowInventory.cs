using System.Threading.Tasks;
using MMOTFG_Bot.Inventory;

namespace MMOTFG_Bot.Communicator
{
    /// <summary>
    /// Shows the inventory of the player.
    /// </summary>
    class cShowInventory : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Lists all items in the player's inventory
Use: inventory";
        }
        public override void SetKeywords()
        {
            key_words = new string[] {
                "inventory",
                "items",
                "inv"
            };
        }

        internal override async Task Execute(string command, string chatId, string[] args = null)
        {
            await InventorySystem.ShowInventory(chatId);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /show_inventory
            if (args.Length != 0) return false;

            return true;
        }
    }
}
