using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MMOTFG_Bot.Items;
using MMOTFG_Bot.Inventory;

namespace MMOTFG_Bot.Commands
{
    //TO-DO: ESTO ESTÁ SIN TERMINAR. NO ES DEFINITIVO.
    class cEquipItem : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Equips an item on its gear slot.
Use: equip [item name]";
        }
        public override void SetKeywords()
        {
            key_words = new string[]{
                "equip"
            };
        }

        internal override async Task Execute(string command, string chatId, string[] args = null)
        {
            if (InventorySystem.StringToItem(args[0], out ObtainableItem item))
            {
                EquipableItem eItem = (EquipableItem)item;
                await InventorySystem.EquipGear(chatId, eItem);
            }
            else await TelegramCommunicator.SendText(chatId, "The specified item doesn't exist");
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /consume itemName
            if (args.Length != 1) return false;

            return true;
        }
    }
}
