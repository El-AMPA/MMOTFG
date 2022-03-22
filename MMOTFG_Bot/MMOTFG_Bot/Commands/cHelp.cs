using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
	class cHelp : ICommand
	{
		public override void setDescription()
		{
			commandDescription = @"Help se usa para obtener la lista de comandos o informacion sobre un comando en concreto
Uso: help [nombre del comando]";
		}


		public override void SetKeywords()
		{
			key_words = new string[] {
				"help"
			};

		}

		internal async override void Execute(string command, long chatId, string[] args = null)
		{
			string response = "";

			if (args.Length == 0)
			{

				response = "Lista de comandos disponibles:\n";

				foreach (ICommand c in Program.commandList)
				{
					string sameCommand = "";

					foreach (string comStr in c.getKeywords())
					{
						sameCommand = sameCommand + "  " + comStr;
					}

					response += sameCommand + "\n";
				}

				response += @"
Para mas informacion sobre un comando concreto, usa:
help [nombre comando]";

				await TelegramCommunicator.SendText(chatId, response);
			}
			else if (args.Length == 1)
			{
				foreach (ICommand c in Program.commandList)
				{
					if (c.ContainsKeyWord(args[0]))
					{
						response = c.getDescription();
						await TelegramCommunicator.SendText(chatId, response);
						return;
					}
				}
				response = "That command doesn't exist";
				await TelegramCommunicator.SendText(chatId, response);
			}
			
		}

		internal override bool IsFormattedCorrectly(string[] args)
		{
			if (args.Length < 2) return true;
			return false;
		}
	}
}
