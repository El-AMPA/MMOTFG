using System.Threading.Tasks;
using MMOTFG_Bot.Inventory;

namespace MMOTFG_Bot.Commands
{
    class cUnequipItem : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Unequips an item from it's gear slot.
Use: unequip [item name] / unequip [gear slot]";
        }
        public override void SetKeywords()
        {
            key_words = new string[]{
                "unequip"
            };
        }

        internal override async Task Execute(string command, string chatId, string[] args = null)
        {
            //Check if the args provided is an equipment slot
            if (InventorySystem.StringToEquipmentSlot(args[0], out EQUIPMENT_SLOT slot))
            {
                //If it's valid, unequip the item from that given slot.
                await InventorySystem.UnequipGear(chatId, slot);
            }
            else
            {
                //Check if the args provided is an equippable item
                if (InventorySystem.StringToItem(args[0], out ObtainableItem item))
                {
                    //If it's valid, unequip the item from the player's gear
                    await InventorySystem.UnequipGear(chatId, (EquipableItem)item);
                }
                else await TelegramCommunicator.SendText(chatId, "The specified equippable item or gear slot doesn't exist");
            }
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /unequip gear_slot || /unequip equippable_item
            if (args.Length != 1) return false;

            return true;
        }
    }
}
