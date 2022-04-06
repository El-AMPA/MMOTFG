using System;
using System.Collections.Generic;
using System.Text;
using static MMOTFG_Bot.StatName;

namespace MMOTFG_Bot.Items
{
    class ManaPotion : ObtainableItem
    {
        public ManaPotion()
        {
            name = "mana_potion";
            maxStackQuantity = 3;
        }

        private async void DrinkPotion(string chatId, string[] args)
        {
            await BattleSystem.changePlayerStats(chatId, MP, 500);
            await TelegramCommunicator.SendText(chatId, "Gained 5MP.");
        }

        private async void EatPotion(string chatId, string[] args)
        {
            await BattleSystem.changePlayerStats(chatId, MP, -2);
            await TelegramCommunicator.SendText(chatId, "Why would you eat a potion you freak.\n Your MP has been reduced by 2.");
        }

        public override void Init()
        {
            key_words.Add(new KeyValuePair<string, Action<string, string[]>>("drink", DrinkPotion));
            key_words.Add(new KeyValuePair<string, Action<string, string[]>>("eat", EatPotion));
        }
    }
}
