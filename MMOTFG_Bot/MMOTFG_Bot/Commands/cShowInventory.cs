using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    class cShowInventory : ICommand
    {
        public override void Init()
        {
            key_words = new string[] {
                "/show_inventory"
            };
        }

        internal override void Execute(string command, long chatId, string[] args = null)
        {
            InventorySystem.ShowInventory(chatId);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /show_inventory
            if (args.Length != 0) return false;

            return true;
        }
    }
}
