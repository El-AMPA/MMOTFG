using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    class cShowStats : ICommand
    {
        public override void SetKeywords()
        {
            key_words = new string[] {
                "/stats"
            };
        }

        internal override void Execute(string command, long chatId, string[] args = null)
        {
            BattleSystem.showStats(chatId);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /stats
            if (args.Length != 0) return false;

            return true;
        }
    }
}
