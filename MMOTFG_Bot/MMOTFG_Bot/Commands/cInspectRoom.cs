using System;
using System.Collections.Generic;
using System.Text;
using MMOTFG_Bot.Navigation;

namespace MMOTFG_Bot.Commands
{
    class cInspectRoom : ICommand
    {
        public override void Init()
        {
            key_words = new string[] {
                "/inspect",
                "/look_around",
                "/look"
            };
        }

        internal override void Execute(string command, long chatId, string[] args = null)
        {
            Map.OnInspect(chatId);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            if (args.Length == 0) return true;
            return false;
        }
    }
}
