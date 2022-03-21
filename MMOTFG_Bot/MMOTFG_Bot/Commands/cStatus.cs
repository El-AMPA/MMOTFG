using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    class cStatus : ICommand
    {
        public override void SetKeywords()
        {
            key_words = new string[] {
                "status",
                "s"
            };
        }

        internal override void Execute(string command, long chatId, string[] args = null)
        {
            if (args.Length == 0) BattleSystem.showStatus(chatId, BattleSystem.player);
            else BattleSystem.showStatus(chatId, BattleSystem.enemy);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /status enemy (optional)
            if (args.Length > 1) return false;

            return true;
        }
    }
}
