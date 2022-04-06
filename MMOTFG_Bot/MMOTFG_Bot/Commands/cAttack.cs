using System;
using System.Collections.Generic;
using System.Linq;
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

        internal override async Task Execute(string command, string chatId, string[] args = null)
        {
            if (BattleSystem.player.learningAttack != null)
            {
                await BattleSystem.player.ForgetAttack(chatId, command);
            }
            else
            {
                if (args.Length == 0)
                {
                    await BattleSystem.PlayerAttack(chatId, command);
                }
                else await BattleSystem.PlayerAttack(chatId, command, args[0]);
            }
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: attackName target (optional)
            return true;
        }
    }
}
