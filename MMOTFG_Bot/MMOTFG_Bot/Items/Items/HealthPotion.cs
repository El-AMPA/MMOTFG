using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class HealthPotion : ObtainableItem
    {
        public HealthPotion()
        {
            name = "health_potion";
            maxStackQuantity = 5;
            iD = Guid.NewGuid();
        }

        private void drinkPotion(long chatId, string[] args)
        {
            TelegramCommunicator.SendText(chatId, "Gained 5HP.");
        }

        private void eatPotion(long chatId, string[] args)
        {
            TelegramCommunicator.SendText(chatId, "Why would you eat a potion you freak.\n Your health has been reduced by 2 points." +
                ". As if we had an HP system lmao.");
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
            foreach(KeyValuePair<string, Action<long, string[]>> a in key_words)
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
