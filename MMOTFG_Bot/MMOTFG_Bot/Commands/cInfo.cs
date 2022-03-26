using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Commands
{
    class cInfo : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Obtiene informacion sobre un objeto, estadistica, etcetera.
Uso: info [cosa para la que quieres mas info]";
        }
        public override void SetKeywords()
        {
            key_words = new string[] {
                "info"
            };
        }

        internal override async Task Execute(string command, long chatId, string[] args = null)
        {
            await InformationSystem.ShowInfo(chatId, args[0]);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /info object
            if (args.Length != 1) return false;

            return true;
        }
    }
}
