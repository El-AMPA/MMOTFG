using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types.Enums;

namespace MMOTFG_Bot.Commands
{
    class cTestHyperlink : ICommand
    {
        public override void SetKeywords()
        {
            key_words = new string[] {
                "test"
            };
        }

        internal async override void Execute(string command, long chatId, string[] args = null)
        {
            //habría que preguntar al mapa qué enemigo hay en esta sala
            await TelegramCommunicator.SendText(chatId, "/go_north");
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: attackName
            return true;
        }
    }
}
