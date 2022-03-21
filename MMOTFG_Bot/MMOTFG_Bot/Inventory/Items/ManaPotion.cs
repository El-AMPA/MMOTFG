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

        private async void drinkPotion(long chatId, string[] args)
        {
            BattleSystem.player.changeStat(MP, 5);
            await TelegramCommunicator.SendText(chatId, "Gained 5MP.");
        }

        private async void eatPotion(long chatId, string[] args)
        {
            BattleSystem.player.changeStat(MP, -2);
            await TelegramCommunicator.SendText(chatId, "Why would you eat a potion you freak.\n Your MP has been reduced by 2.");
        }

        public override void Init()
        {
            key_words.Add(new KeyValuePair<string, Action<long, string[]>>("drink", drinkPotion));
            key_words.Add(new KeyValuePair<string, Action<long, string[]>>("eat", eatPotion));
        }
    }
}
