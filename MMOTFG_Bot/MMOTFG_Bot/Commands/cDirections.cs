using System;
using System.Collections.Generic;
using System.Text;
using MMOTFG_Bot.Navigation;

namespace MMOTFG_Bot.Commands
{
    class cDirections : ICommand
    {
        public override void Init()
        {
            key_words = new string[] {
                "/directions"
            };
        }

        internal override void Execute(string command, long chatId, string[] args = null)
        {
            Map.GetDirections(chatId);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            if (args.Length == 0) return true;
            return false;
        }
    }
}
