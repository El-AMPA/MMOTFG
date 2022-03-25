using System;
using System.Collections.Generic;
using System.Text;
using MMOTFG_Bot.Items;

namespace MMOTFG_Bot.Commands
{
    class cUnequipItem : ICommand
    {
        public override void setDescription()
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

        internal async override void Execute(string command, long chatId, string[] args = null)
        {
            EQUIPMENT_SLOT slot;
            //Check if the args provided is an equipment slot
            if (InventorySystem.StringToEquipmentSlot(args[0], out slot))
            {
                //If it's valid, unequip the item from that given slot.
                await InventorySystem.unequipGear(chatId, slot);
            }
            else
            {
                ObtainableItem item;
                //Check if the args provided is an equippable item
                if (InventorySystem.StringToItem(args[0], out item))
                {
                    //If it's valid, unequip the item from the player's gear
                    await InventorySystem.unequipGear(chatId, (EquipableItem)item);
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
