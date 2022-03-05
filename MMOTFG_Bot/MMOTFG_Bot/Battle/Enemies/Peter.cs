using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Battle.Enemies
{
    class Peter : Enemy
    {
        public Peter()
        {
            name = "\U0001F171eter";

            imageName = "peter.png";
            imageCaption = "\U0001F171eter te mira ominosamente...";
            droppedMoney = 1000000;
            droppedItem = new Items.ThunderfuryBleesedBladeOfTheWindseeker();
            droppedItemAmount = 1;

            stats = new float[]{50, 1, 100};
            originalStats = (float[])stats.Clone();

            attacks = new Attack[]{
                new ScaledAttack("PeterTorta", 1, 0)
            };

            attackNum = attacks.Length;
        }

        public override async void OnHit(long chatId)
        {
            if (stats[(int)StatName.HP] < 25)
            {
                stats[(int)StatName.ATK] *= 50;
                await TelegramCommunicator.SendText(chatId, "\U0001F171eter says: \"Nothin personnel, kid\"");
            }
        }
    }
}
