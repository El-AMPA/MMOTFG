using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Battle.Enemies
{
    class Error : Enemy
    {
        public Error()
        {
            name = "ERROR";

            stats = new float[]{1000000, 1000000, 10000000};
            originalStats = (float[])stats.Clone();

            attacks = new Attack[]{
                new Attack("DIE", 1000000, 0)
            };

            attackNum = attacks.Length;
        }

        public override async void OnHit(long chatId)
        {
            await TelegramCommunicator.SendText(chatId, "ESTO ES UN ERROR. NO DEBERÍA ESTAR AQUÍ");
        }
    }
}
