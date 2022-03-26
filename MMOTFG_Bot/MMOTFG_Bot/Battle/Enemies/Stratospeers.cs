using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Battle.Enemies
{
    class Stratospeers : Enemy
    {
        public Stratospeers()
        {
            name = "Stratospeers";

            imageName = "stratospeers.png";
            imageCaption = "¡Los astronautas del PCE pasan de convivir contigo!";
            droppedMoney = 1920;
            droppedItem = new Items.HealthPotion();
            droppedItemAmount = 2;

            stats = new float[]{40, 10, 10};
            originalStats = (float[])stats.Clone();

            attacks = new Attack[]{
                new Attack("Ondas del wifi", 1, 0),
                new Attack("La Chancla", 3, 3),
                new Attack("Modo capitalismo", 5, 5)
            };

            attackNum = attacks.Length;
        }

        public override async Task OnHit(long chatId)
        {
            if (stats[(int)StatName.HP] < originalStats[(int)StatName.HP] / 4)
            {
                stats[(int)StatName.ATK] /= 4;
                await TelegramCommunicator.SendText(chatId, "¡Ya solo queda uno! Habrá ganado la partida del PCE, pero no la batalla contigo.");
            }
        }
    }
}