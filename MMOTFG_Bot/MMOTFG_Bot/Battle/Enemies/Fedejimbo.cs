using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Battle.Enemies
{
    class Fedejimbo : Enemy
    {
        public Fedejimbo()
        {
            name = "Fedejimbo";

            imageName = "fedejimbo.png";
            imageCaption = "¡Fedejimbo no te deja pasar de esta esquina!";
            droppedMoney = 666;
            droppedItem = new Items.HealthPotion();
            droppedItemAmount = 2;

            stats = new float[]{100, 10, 25};
            originalStats = (float[])stats.Clone();

            attacks = new Attack[]{
                new Attack("Fantasma de la ópera", 5, 5),
                new Attack("Deconstructción", 2, 1),
                new Attack("Chapa en PowerPoint", 1, 0)
            };

            attackNum = attacks.Length;
        }

        public override async void OnHit(long chatId)
        {
            if (stats[(int)StatName.HP] < originalStats[(int)StatName.HP] / 2)
            {
                stats[(int)StatName.MP] = originalStats[(int)StatName.MP];
                await TelegramCommunicator.SendText(chatId, "¡Fedejimbo conjura un patrón de diseño arcano de su libro y se recuperan todos sus MP!");
            }
        }
    }
}
