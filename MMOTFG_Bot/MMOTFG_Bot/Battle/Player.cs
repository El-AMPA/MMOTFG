using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    class Player : Battler
    {
        public List<string> attackNames = new List<string>();
        public List<float> attackmpCosts = new List<float>();

        public Player()
        {
            name = "Player";

            stats = new float[] { 100, 10, 50 };

            attacks = new Attack[]{
                new Attack("Tortazo", 1.5f, 0),
                new Attack("Patada", 2, 1),
                new aStatChanging("Burla", 0, 5),
                new Attack("Overkill", 100, 100)
            };

            foreach(Attack a in attacks)
            {
                attackNames.Add(a.name);
                attackmpCosts.Add(a.mpCost);
            }

            onCreate();
        }

        public override async void OnKill(long chatId)
        {
            //Recuperas toda la vida y mp
            stats[(int)StatName.HP] = originalStats[(int)StatName.HP];
            stats[(int)StatName.MP] = originalStats[(int)StatName.MP];
            await TelegramCommunicator.SendText(chatId, "You died!");
        }

        public void OnBattleOver()
        {
            //reseteamos los stats (excepto HP/MP) a su estado original
            for (int i = 0; i < Stats.statNum; i++)
            {
                StatName sn = (StatName)i;
                if (sn != StatName.HP && sn != StatName.MP)
                    stats[i] = originalStats[i];
            }
        }
    }
}
