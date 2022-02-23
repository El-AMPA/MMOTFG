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

        private async void drinkPotion(long chatId, string[] args)
        {
            await TelegramCommunicator.SendText(chatId, "Gained 5HP.");
        }

        private async void eatPotion(long chatId, string[] args)
        {
            await TelegramCommunicator.SendText(chatId, "Why would you eat a potion you freak.\n Your health has been reduced by 2 points." +
                ". As if we had an HP system lmao.");
        }

        public override void Init()
        {
            key_words.Add(new KeyValuePair<string, Action<long, string[]>>("/drink", drinkPotion));
            key_words.Add(new KeyValuePair<string, Action<long, string[]>>("/eat", eatPotion));
        }
    }
}
