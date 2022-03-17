using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Battle.Enemies
{
    class Manuela : Enemy
    {
        public Manuela()
        {
            name = "Manuela";

            imageName = "manuela.jpg";
            imageCaption = "Manuela ataca!";
            droppedMoney = 100;
            droppedItem = new Items.HealthPotion();
            droppedItemAmount = 2;

            stats = new float[]{50, 10, 25};
            
            attacks = new Attack[]{
                new Attack("Arañazo", 1, 0),
                new Attack("Super Arañazo", 2, 1)
            };

            onCreate();
        }

        public override async void OnHit(long chatId)
        {
            if (stats[(int)StatName.HP] < 25)
            {
                stats[(int)StatName.ATK] *= 2;
                await TelegramCommunicator.SendText(chatId, "Manuela se ha enfadado! Sus ataques harán más daño.");
            }
        }
    }
}
