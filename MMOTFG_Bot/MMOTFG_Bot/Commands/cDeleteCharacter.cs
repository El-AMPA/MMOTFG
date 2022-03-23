using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{/// <summary>
 /// Shows the available directions from a given node.
 /// </summary>
	class cDeleteCharacter : ICommand
	{
		public override void setDescription()
		{
			commandDescription = @"Borra tu personaje PERMANENTEMENTE. Esta accion no se puede deshacer.
Uso: reset [nombre del personaje a borrar]";
		}

		public override void SetKeywords()
		{
			key_words = new string[] {
				"reset"
			};
		}

		async internal override void Execute(string command, long chatId, string[] args = null)
		{			

			string arg0 = args[0];
			string arg1 = args[1];

			Dictionary<string, object> tempDict = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID, chatId.ToString(), DbConstants.COLLEC_DEBUG);

			if (arg0 == tempDict[DbConstants.PLAYER_FIELD_NAME].ToString())
			{
				await DatabaseManager.DeleteDocumentById(chatId.ToString(), DbConstants.COLLEC_DEBUG);
				await TelegramCommunicator.SendText(chatId, "hasta siempre 😭");
			}else
			{
				await TelegramCommunicator.SendText(chatId, "Uso: reset [nombre del personaje a borrar] [nombre del personaje a borrar]");
			}
				

		}

		internal override bool IsFormattedCorrectly(string[] args)
		{
			if (args.Length == 1) return true;
			return false;
		}
	}
}
