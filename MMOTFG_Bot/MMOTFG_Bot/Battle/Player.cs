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

            originalStats = (float[])stats.Clone();

            attacks = new Attack[]{
                new Attack("Tortazo", 1.5f, 0),
                new Attack("Patada", 2, 1),
                new StatReducingAttack("Burla", 0, 5),
                new Attack("Overkill", 100, 100)
            };

            attackNum = attacks.Length;
            foreach(Attack a in attacks)
            {
                attackNames.Add(a.name);
                attackmpCosts.Add(a.mpCost);
            }
        }

        public override async void OnKill(long chatId)
        {
            await TelegramCommunicator.SendText(chatId, "You died!");
        }
    }
}
