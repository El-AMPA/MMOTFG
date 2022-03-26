using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MMOTFG_Bot.Navigation;

namespace MMOTFG_Bot.Commands
{
    /// <summary>
    /// Sends the 'OnInspectText' field of the current node of the player 
    /// </summary>
    class cInspectRoom : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Inspects the current room
Use: inpect";
        }
        public override void SetKeywords()
        {
            key_words = new string[] {
                "inspect",
                "look",
                "ins",
                "i"
            };
        }

        internal override async Task Execute(string command, long chatId, string[] args = null)
        {
            await Map.OnInspect(chatId);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            if (args.Length == 0) return true;
            return false;
        }
    }
}
