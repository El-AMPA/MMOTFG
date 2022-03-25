using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Battle.Enemies
{
    class Sein : Enemy
    {
        public Sein()
        {
            name = "Sein";

            imageName = "sein.png";
            imageCaption = "¡Sein quiere jugar a pegarse!";
            droppedMoney = 6969;
            droppedItem = "health_potion";
            droppedItemAmount = 2;

            stats = new float[]{100, 10, 25};
            originalStats = (float[])stats.Clone();

            attacks = new Attack[]{
                new Attack("Plástico sospechoso", 2, 1),
                new Attack("Latigazo correa", 1, 0)
            };

            attackNum = attacks.Length;
        }

        public override async void OnHit(long chatId)
        {
            if (stats[(int)StatName.HP] < originalStats[(int)StatName.HP] / 2)
            {
                stats[(int)StatName.ATK] /=2;
                await TelegramCommunicator.SendText(chatId, "Sein se está cansando de jugar contigo y notas que pronto va a dejar de hacerte caso.");
            }
        }
    }
}
