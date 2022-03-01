﻿using System;
using System.Collections.Generic;
using System.Text;
using MMOTFG_Bot.Navigation;

namespace MMOTFG_Bot.Commands
{
    class cNavigate : ICommand
    {
        public override void Init()
        {
            key_words = new string[]{
                "/go",
            };
        }

        internal override void Execute(string command, long chatId, string[] args = null)
        {
            Map.Navigate(chatId, args[0]);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            if (args.Length != 1) return false;

            return true;
        }
    }
}
