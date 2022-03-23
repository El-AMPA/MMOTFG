using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    class cAttack : ICommand
    {
        public override void setDescription()
        {
            commandDescription = @"No hay info de este comando";
        }
        public override void SetKeywords()
        {
            //las keywords son los ataques que tenga el jugador
        }

        public void setKeywords(string[] kw)
        {
            key_words = kw;
        }

        internal override void Execute(string command, long chatId, string[] args = null)
        {
            //habría que preguntar al mapa qué enemigo hay en esta sala
            BattleSystem.playerAttack(chatId, command);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: attackName
            return true;
        }
    }
}
