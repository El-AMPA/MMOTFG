﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
	class cHelp : ICommand
	{
		public override void setDescription()
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

		internal async override void Execute(string command, long chatId, string[] args = null)
		{
			string response = "";

			if (args.Length == 0)
			{

				response = "List of available commands with its synonyms:\n";

				foreach (ICommand c in Program.commandList)
				{
					string sameCommand = "/help_";

					foreach (string comStr in c.getKeywords())
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
