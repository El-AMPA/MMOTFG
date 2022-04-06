using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Commands
{/// <summary>
 /// Shows the available directions from a given node.
 /// </summary>
	class cDebug : ICommand
	{
		public override void SetDescription()
		{
			commandDescription = @"Used for debugging. Only for lazy devs :)";
		}

		public override void SetKeywords()
		{
			key_words = new string[] {
				"debug"
			};
		}

		internal override async Task Execute(string command, string chatId, string[] args = null)
		{			

			string arg0 = args[0];

			switch (arg0.ToLower())
			{
				case "resetinv":
					await InventorySystem.CreatePlayerInventory(chatId);
					Console.WriteLine("--- using debug command {0} ---", arg0);
					break;
				case "delete":
					await DatabaseManager.DeleteDocumentById(chatId.ToString(), DbConstants.COLLEC_DEBUG);
					Console.WriteLine("--- using debug command {0} ---", arg0);
					await TelegramCommunicator.SendText(chatId, "hasta siempre 😭");
					break;
			}
				

		}

		internal override bool IsFormattedCorrectly(string[] args)
		{
			if (args.Length > 0) return true;
			return false;
		}
	}
}
