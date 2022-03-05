using MMOTFG_Bot.Battle.Enemies;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    class cFight : ICommand
    {
        public override void SetKeywords()
        {
            key_words = new string[] {
                "/fight"
            };
        }

        internal override void Execute(string command, long chatId, string[] args = null)
        {
            //habría que preguntar al mapa qué enemigo hay en esta sala
            BattleSystem.startBattle(chatId, new Manuela());
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /fight
            return true;
        }
    }
}
