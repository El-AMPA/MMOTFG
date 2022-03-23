using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    /// <summary>
    /// Shows the inventory of the player.
    /// </summary>
    class cShowInventory : ICommand
    {
        public override void setDescription()
        {
            commandDescription = @"Muestra el inventario del jugador
Uso: inventory / show_inventory / items";
        }
        public override void SetKeywords()
        {
            key_words = new string[] {
                "inventory",
                "items",
                "inv"
            };
        }

        internal override async void Execute(string command, long chatId, string[] args = null)
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
