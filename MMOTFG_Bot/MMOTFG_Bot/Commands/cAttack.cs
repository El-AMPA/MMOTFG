using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Commands
{
    class cAttack : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"There is no specific information on this command";
        }
        public override void SetKeywords()
        {
            //las keywords son los ataques que tenga el jugador
        }

        public void SetKeywords(string[] kw)
        {
            key_words = kw;
        }

        internal override async Task Execute(string command, long chatId, string[] args = null)
        {
            //habría que preguntar al mapa qué enemigo hay en esta sala
            await BattleSystem.PlayerAttack(chatId, command);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: attackName
            return true;
        }
    }
}
