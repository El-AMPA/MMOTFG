using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Commands
{/// <summary>
 /// Shows the available directions from a given node.
 /// </summary>
	class cDeleteCharacter : ICommand
	{
		public override void SetDescription()
		{
			commandDescription = @"Deletes PERMANENTLY your character.
Use: delete [character name]";
		}

		public override void SetKeywords()
		{
			key_words = new string[] {
				"delete"
			};
		}

		internal override async Task Execute(string command, string chatId, string[] args = null)
		{			

			string arg0 = args[0];

			//delete all
			if(arg0 == "all")
            {
				await DatabaseManager.DeleteCollection(DbConstants.COLLEC_DEBUG);
			}
			//delete one player
            else
            {
				Dictionary<string, object> tempDict = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID, chatId.ToString(), DbConstants.COLLEC_DEBUG);

				if (tempDict != null && arg0 == tempDict[DbConstants.PLAYER_FIELD_NAME].ToString())
				{
					await DatabaseManager.DeleteDocumentById(chatId.ToString(), DbConstants.COLLEC_DEBUG);
					await Program.Communicator.SendText(chatId, "Bye 😭");
				}
				else
				{
					await Program.Communicator.SendText(chatId, "Use: delete [character name]. You can only delete you own character!");
				}
			}
		}

		internal override bool IsFormattedCorrectly(string[] args)
		{
			if (args.Length == 1) return true;
			return false;
		}
	}
}
