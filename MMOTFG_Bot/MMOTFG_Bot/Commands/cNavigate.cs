﻿using System;
using System.Collections.Generic;
using System.Text;
using MMOTFG_Bot.Navigation;

namespace MMOTFG_Bot.Commands
{
    /// <summary>
    /// Moves the player in the specified direction
    /// </summary>
    class cNavigate : ICommand
    {
        public override void setDescription()
        {
            commandDescription = @"Te mueves en la dirección indicada. Para una lista de estas direcciones, usa /directions
Uso: go [direccion]";
        }
        public override void SetKeywords()
        {
            key_words = new string[]{
                "go",
                "g"
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
