﻿using System;
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
                "/equip"
            };
        }

        //TO-DO: como usan cosas async a lo mejor los execute tendrían que ser async
        internal async override void Execute(string command, long chatId, string[] args = null)
        {
            ObtainableItem item;
            if (InventorySystem.StringToItem(args[0], out item))
            {
                EquipableItem eItem = (EquipableItem)item; //lol. Si funciona doy gracias. O pido perdon.
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
