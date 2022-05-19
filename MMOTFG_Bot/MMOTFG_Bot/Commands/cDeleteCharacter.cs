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

			Dictionary<string, object> tempDict = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID, chatId, DbConstants.COLLEC_PLAYERS);

			if (tempDict != null && arg0 == tempDict[DbConstants.PLAYER_FIELD_NAME].ToString())
			{
				bool inParty = await PartySystem.IsInParty(chatId);
				if (inParty) await PartySystem.ExitParty(chatId);
				await DatabaseManager.DeleteDocumentById(chatId, DbConstants.COLLEC_PLAYERS);
				await TelegramCommunicator.SendText(chatId, "Bye 😭");
			}else
			{
				await TelegramCommunicator.SendText(chatId, "Use: delete [character name]. You can only delete you own character!");
			}
				

		}

		internal override bool IsFormattedCorrectly(string[] args)
		{
			if (args.Length == 1) return true;
			return false;
		}
	}
}
