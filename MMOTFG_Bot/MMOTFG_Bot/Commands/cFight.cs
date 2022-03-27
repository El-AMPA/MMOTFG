using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Commands
{
    class cFight : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"There is no specific information on this command";
        }
        public override void SetKeywords()
        {
            key_words = new string[] {
                "fight"
            };
        }

        internal override async Task Execute(string command, long chatId, string[] args = null)
        {
            //habría que preguntar al mapa qué enemigo hay en esta sala
            await BattleSystem.startBattle(chatId, JSONSystem.getEnemy("Manuela"));
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /fight
            return true;
        }
    }
}
