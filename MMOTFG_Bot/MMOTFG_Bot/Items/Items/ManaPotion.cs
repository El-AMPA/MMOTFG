﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class ManaPotion : ObtainableItem
    {
        public ManaPotion()
        {
            name = "mana_potion";
            maxStackQuantity = 3;
            iD = Guid.NewGuid();
        }

        private void drinkPotion(long chatId, string[] args)
        {
            TelegramCommunicator.SendText(chatId, "Gained 5MP.");
        }

        private void eatPotion(long chatId, string[] args)
        {
            TelegramCommunicator.SendText(chatId, "Why would you eat a potion you freak.\n Your MP has been reduced by 2." +
                ". As if we had an MP system lmao.");
        }

        public override void Init()
        {
            key_words = new KeyValuePair<string, Action<long, string[]>>[]
            {
                new KeyValuePair<string, Action<long, string[]>>("/drink", drinkPotion),
                new KeyValuePair<string, Action<long, string[]>>("/eat", eatPotion)
            };
        }

        public override bool ProcessCommand(string command, long chatId, string[] args = null)
        {
            foreach (KeyValuePair<string, Action<long, string[]>> a in key_words)
            {
                if (a.Key == command)
                {
                    a.Value(chatId, args);
                    return true;
                }
            }
            return false;
        }
    }
}
