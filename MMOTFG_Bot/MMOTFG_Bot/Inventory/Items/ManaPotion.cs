using System;
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

        private async void drinkPotion(long chatId, string[] args)
        {
            await TelegramCommunicator.SendText(chatId, "Gained 5MP.");
        }

        private async void eatPotion(long chatId, string[] args)
        {
            await TelegramCommunicator.SendText(chatId, "Why would you eat a potion you freak.\n Your MP has been reduced by 2." +
                " As if we had an MP system lmao.");
        }

        public override void Init()
        {
            key_words.Add(new KeyValuePair<string, Action<long, string[]>>("/drink", drinkPotion));
            key_words.Add(new KeyValuePair<string, Action<long, string[]>>("/eat", eatPotion));
        }
    }
}
