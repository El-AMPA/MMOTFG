using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    class cHelp : ICommand
    {
        public override void SetKeywords()
        {
            key_words = new string[] {
                "/help"
            };
        }

        internal async override void Execute(string command, long chatId, string[] args = null)
        {
            string response = "List of available commands:\n";

            foreach(ICommand c in Program.commandList)
			{
                string sameCommand = "";

				foreach (string comStr in c.getKeywords())
				{
                    sameCommand = sameCommand + "  " + comStr;
				}

                response = response + sameCommand + "\n";
			}

            await TelegramCommunicator.SendText(chatId, response);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /help
            
            return true;
        }
    }
}
