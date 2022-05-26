using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Commands
{
    class cFight : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Starts a battle with the specified enemy or enemies.
If in a party, can only be used by the leader.
Use: fight [enemyName] [enemyName2] [enemyName3]...";
        }

        public override void SetKeywords()
        {
            key_words = new string[] {
                "fight"
            };
        }

        internal override async Task Execute(string command, string chatId, string[] args = null)
        {
            await BattleSystem.StartBattleFromNames(chatId, args.ToList());
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: fight [enemyName] [enemyName]...
            return args.Length > 0;
        }
    }
}
