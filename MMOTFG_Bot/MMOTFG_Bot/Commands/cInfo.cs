using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    class cInfo : ICommand
    {
        public override void SetKeywords()
        {
            key_words = new string[] {
                "info"
            };
        }

        internal override void Execute(string command, long chatId, string[] args = null)
        {
            InformationSystem.showInfo(chatId, args[0]);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /info object
            if (args.Length != 1) return false;

            return true;
        }
    }
}
