using System;
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
            commandDescription = @"No hay info de este comando";
        }
        public override void SetKeywords()
        {
            key_words = new string[]{
                "go",
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
