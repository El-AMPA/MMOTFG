using System;
using System.Collections.Generic;
using System.Text;
using MMOTFG_Bot.Items;

namespace MMOTFG_Bot.Commands
{
    //TO-DO: ESTO ESTÁ SIN TERMINAR. NO ES DEFINITIVO.
    class cEquipItem : ICommand
    {
        public override void SetKeywords()
        {
            key_words = new string[]{
                "/equip",
            };
        }

        internal override void Execute(string command, long chatId, string[] args = null)
        {
            ObtainableItem item;
            if (InventorySystem.StringToItem(args[0], out item))
            {
                EquipableItem eItem = (EquipableItem)item; //lol. Si funciona doy gracias. O pido perdon.
                InventorySystem.EquipGear(chatId, eItem);
            }
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /consume itemName
            if (args.Length != 1) return false;

            return true;
        }
    }
}
