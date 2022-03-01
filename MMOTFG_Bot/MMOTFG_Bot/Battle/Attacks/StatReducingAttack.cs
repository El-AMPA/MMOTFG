using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    class StatReducingAttack : Attack
    {
        public StatReducingAttack(string name_, float power_, float mpCost_) : base(name_, power_, mpCost_) { }

        public override async void OnAttack(long chatId) 
        {
            await TelegramCommunicator.SendText(chatId, $"{target.name}'s attack was lowered!");
            target.stats[(int)StatName.ATK] /= 2;
        }
    }
}
