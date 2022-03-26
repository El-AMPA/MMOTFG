using System;
using System.Collections.Generic;
using System.Text;
using static MMOTFG_Bot.StatName;

namespace MMOTFG_Bot
{
    class StatReducingAttack : Attack
    {
        public StatReducingAttack(string name_, float power_, float mpCost_) : base(name_, power_, mpCost_) { }

        public override async void OnAttack(long chatId) 
        {
            await TelegramCommunicator.SendText(chatId, $"{target.name}'s attack was lowered!");
            target.SetStat(ATK, target.GetStat(ATK)/2);
        }
    }
}
