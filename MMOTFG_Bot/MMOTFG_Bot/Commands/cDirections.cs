using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MMOTFG_Bot.Navigation;

namespace MMOTFG_Bot.Commands
{
    /// <summary>
    /// Shows the available directions from a given node.
    /// </summary>
    class cDirections : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Lists all available directions from your current room
Use: directions";
        }
        public override void SetKeywords()
        {
            key_words = new string[] {
                "directions",
                "dir",
                "d"
            };
        }

        internal override async Task Execute(string command, string chatId, string[] args = null)
        {
            await Map.GetDirections(chatId);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            if (args.Length == 0) return true;
            return false;
        }
    }
}
