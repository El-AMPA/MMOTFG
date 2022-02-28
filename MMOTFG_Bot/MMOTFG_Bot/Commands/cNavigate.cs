using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    class cNavigate : ICommand
    {
        public override void Init()
        {
            throw new NotImplementedException();
        }

        internal override void Execute(string command, long chatId, string[] args = null)
        {
            throw new NotImplementedException();
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            if (args.Length != 2) return false;

            return true;
        }
    }
}
