using System;
using System.Threading.Tasks;
using MMOTFG_Bot.Persistency;
using MMOTFG_Bot.Inventory;

namespace MMOTFG_Bot.Communicator
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
					await DatabaseManager.DeleteDocumentById(chatId, DbConstants.COLLEC_PLAYERS);
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
