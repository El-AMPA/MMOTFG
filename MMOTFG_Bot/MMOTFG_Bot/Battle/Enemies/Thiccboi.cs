using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Battle.Enemies
{
    class Thiccboi : Enemy
    {
        public Thiccboi()
        {
            name = "Thiccboi";

            imageName = "thiccboi.png";
            imageCaption = "¡El thiccboi abre la boca!";
            droppedMoney = 345;
            droppedItem = new Items.HealthPotion();
            droppedItemAmount = 2;

            stats = new float[]{100, 10, 25};
            originalStats = (float[])stats.Clone();

            attacks = new Attack[]{
                new Attack("ñom", 1, 0),
                new Attack("Barras (de pan)", 2, 1),
                new Attack("Ahora puedes jugar a Blown to Blacksmithereens desde el navegador en http://www.tai1games.itch.io", 3, 3)
            };

            attackNum = attacks.Length;
        }

        public override async Task OnHit(long chatId)
        {
            if (stats[(int)StatName.HP] < originalStats[(int)StatName.HP] / 2)
            {
                stats[(int)StatName.HP] -= originalStats[(int)StatName.HP] / 4;
                await TelegramCommunicator.SendText(chatId, "El thiccboi intenta comerse una silla pero no sale bien...");
            }
        }
    }
}
