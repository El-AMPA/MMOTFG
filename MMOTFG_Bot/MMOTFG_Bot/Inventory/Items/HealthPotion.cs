using System;
using System.Collections.Generic;
using System.Text;
using static MMOTFG_Bot.StatName;

namespace MMOTFG_Bot.Items
{
    class HealthPotion : ObtainableItem
    {
        public HealthPotion()
        {
            name = "health_potion";
            maxStackQuantity = 5;
        }

        private async void DrinkPotion(long chatId, string[] args)
        {
            await BattleSystem.changePlayerStats(chatId, HP, 500);
            await TelegramCommunicator.SendText(chatId, "Gained 500HP.");
        }

        private async void EatPotion(long chatId, string[] args)
        {
            await BattleSystem.changePlayerStats(chatId, HP, -2);
            await TelegramCommunicator.SendText(chatId, "Why would you eat a potion you freak.\n Your health has been reduced by 2 points.");
        }

        public override void Init()
        {
            key_words.Add(new KeyValuePair<string, Action<long, string[]>>("drink", DrinkPotion));
            key_words.Add(new KeyValuePair<string, Action<long, string[]>>("eat", EatPotion));
        }
    }
}
