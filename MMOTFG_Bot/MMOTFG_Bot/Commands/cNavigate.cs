using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MMOTFG_Bot.Navigation;

namespace MMOTFG_Bot.Commands
{
    /// <summary>
    /// Moves the player in the specified direction
    /// </summary>
    class cNavigate : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Moves the player in the given direction. For all available directions, use /directions
Use: go [direccion]";
        }
        public override void SetKeywords()
        {
            key_words = new string[]{
                "go",
                "g"
            };
        }

        internal override async Task Execute(string command, long chatId, string[] args = null)
        {
            await Map.Navigate(chatId, args[0]);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            if (args.Length != 1) return false;

            return true;
        }
    }
}
