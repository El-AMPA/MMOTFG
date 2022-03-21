using System;
using System.Collections.Generic;
using System.Text;
using MMOTFG_Bot.Items;

namespace MMOTFG_Bot.Commands
{
    class cShowGear : ICommand
    {
        public override void setDescription()
        {
            commandDescription = @"No hay info de este comando";
        }
        public override void SetKeywords()
        {
            key_words = new string[] {
                "gear",
                "show_gear",
                "equipment"
            };
        }

        internal override async void Execute(string command, long chatId, string[] args = null)
        {
            if (args.Length == 0) await InventorySystem.ShowGear(chatId);
            else {
                EQUIPMENT_SLOT slot;
                InventorySystem.StringToEquipmentSlot(args[0], out slot);
                await InventorySystem.ShowGear(chatId, slot);
            }

        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /gear (weapon) optional
            if (args.Length == 0) return true;

            else if (args.Length == 1 && InventorySystem.StringToEquipmentSlot(args[0], out _)) return true;

            return false;
        }
    }
}
