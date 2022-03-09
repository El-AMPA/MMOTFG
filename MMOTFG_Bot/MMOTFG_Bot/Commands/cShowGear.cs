using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    class cShowGear : ICommand
    {
        public override void SetKeywords()
        {
            key_words = new string[] {
                "/gear",
                "/show_gear",
                "/equipment"
            };
        }

        internal override async void Execute(string command, long chatId, string[] args = null)
        {
            await InventorySystem.ShowGear(chatId);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /show_inventory
            if (args.Length != 0) return false;

            return true;
        }
    }
}
