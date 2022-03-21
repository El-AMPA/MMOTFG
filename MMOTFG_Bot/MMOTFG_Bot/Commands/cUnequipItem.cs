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
            commandDescription = @"No hay info de este comando";
        }
        public override void SetKeywords()
        {
            key_words = new string[]{
                "unequip",
            };
        }

        internal async override void Execute(string command, long chatId, string[] args = null)
        {
            EQUIPMENT_SLOT slot;
            if (InventorySystem.StringToEquipmentSlot(args[0], out slot))
            {
                await InventorySystem.UnequipGear(chatId, slot);
            }
            else await TelegramCommunicator.SendText(chatId, "The specified slot doesn't exist");
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /unequip weapon
            if (args.Length != 1) return false;

            return true;
        }
    }
}
