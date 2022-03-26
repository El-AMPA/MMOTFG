using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Battle.Enemies
{
    class Cleonft : Enemy
    {
        public Cleonft()
        {
            name = "Cleonft";

            imageName = "cleonft.png";
            imageCaption = "¡Cleonft viene a mintearte la carrera!";
            droppedMoney = 2223;
            droppedItem = new Items.HealthPotion();
            droppedItemAmount = 2;

            stats = new float[]{100, 5, 20};
            originalStats = (float[])stats.Clone();

            attacks = new Attack[]{
                new Attack("Chiste sobre javascript", 2, 0),
                new Attack("El PCE es maravilloso", 5, 5),
                new Attack("No Fucking Thanks", 10, 10)
            };

            attackNum = attacks.Length;
        }

        public override async Task OnHit(long chatId)
        {
            var rand = new Random();
            if(rand.Next(2) > 1)
            {
                stats[(int)StatName.ATK] -= 1;
                await TelegramCommunicator.SendText(chatId, "Cleon y el mono se debaten sobre si deberían matarte o no. Su ataque baja un poco.");
            }
        }
    }
}
