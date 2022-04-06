using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Commands
{
	class cHelp : ICommand
	{
		List<ICommand> commandsList;

		public override void SetDescription()
		{
			commandDescription = @"Help is used to list all available commands or look up further information on a specific command
Use: help [command name]";
		}


		public override void SetKeywords()
		{
			key_words = new string[] {
				"help",
				"h"
			};

		}

		internal override async Task Execute(string command, string chatId, string[] args = null)
		{
			string response;

			if (args.Length == 0)
			{

				response = "List of available commands with its synonyms:\n";

				foreach (ICommand c in commandsList)
				{
					string sameCommand = "/help_";

					foreach (string comStr in c.GetKeywords())
					{
						sameCommand = sameCommand + comStr + " ";
					}

					response += sameCommand + "\n";
				}

				response += @"For further information on a given command, please use help [command name]";

				await TelegramCommunicator.SendText(chatId, response);
			}
			else if (args.Length == 1)
			{
				foreach (ICommand c in Program.commandList)
				{
					if (c.ContainsKeyWord(args[0]))
					{
						response = c.GetDescription();
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

		public void setCommandList(List<ICommand> commands)
		{
			commandsList = commands;
		}
	}
}
