using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    class cShowInventory : ICommand
    {
        public string[] palabras_clave =
        {
            "/show_inventory"
        };

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

        public override bool ContainsKeyWord(string command, long chatId, string[] args = null)
        {
            if (!IsFormattedCorrectly(args)) return false;
            foreach (string p in palabras_clave)
            {
                if (command == p) Execute(command, chatId, args);
            }
            return false;
        }
    }
}
