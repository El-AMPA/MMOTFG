using System;
using System.Collections.Generic;
using System.Text;
using MMOTFG_Bot.Items;

namespace MMOTFG_Bot.Commands
{
    class cUnequipItem : ICommand
    {
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
                if(InventorySystem.StringToItem(args[0], out item))
                {
                    await InventorySystem.unequipGear(chatId, (EquipableItem)item);
                }
                else await TelegramCommunicator.SendText(chatId, "The specified item doesn't exist");
            }
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /unequip weapon
            if (args.Length != 1) return false;

            return true;
        }
    }
}
