using System;
using System.Collections.Generic;
using System.Text;
using MMOTFG_Bot.Navigation;

namespace MMOTFG_Bot.Commands
{
	/// <summary>
	/// Shows the available directions from a given node.
	/// </summary>
	class cCreateCharacter : ICommand
	{
		public override void setDescription()
		{
			commandDescription = @"No hay info de este comando";
		}
		public override void SetKeywords()
		{
			key_words = new string[] {
				"create"
			};
		}

		async internal override void Execute(string command, long chatId, string[] args = null)
		{

			string charName = args[0];

			Dictionary<string,object>tempDict = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_NAME, charName, DbConstants.COLLEC_DEBUG);

			if(tempDict != null)
			{
				await TelegramCommunicator.SendText(chatId, "Ese nombre ya esta pillado, sé un poco mas original ;)");
				return;
			}

			tempDict = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID, chatId.ToString(), DbConstants.COLLEC_DEBUG);

			if (tempDict != null)
			{
				await TelegramCommunicator.SendText(chatId, "Solo se puede tener un personaje que la luz esta cara");
				return;
			}

			Dictionary<string, object> dict = new Dictionary<string, object>
			{
				{ DbConstants.PLAYER_FIELD_NAME , charName},
				{ DbConstants.PLAYER_FIELD_TELEGRAM_ID, chatId.ToString()}
			};

			bool created = await DatabaseManager.AddDocumentToCollection(dict, chatId.ToString(), DbConstants.COLLEC_DEBUG);

			if (!created)
			{
				await TelegramCommunicator.SendText(chatId, "Error al crear el personaje");
				Console.WriteLine("Error when trying to add character with telegramId {} to the database", chatId);
				return;
			}

			await TelegramCommunicator.SendText(chatId, String.Format("Madre mia {0}, tremendo personaje acabas de crearte", charName));
			Console.WriteLine("Telegram user {0} just created characater with name {1}", chatId, charName);

			await InventorySystem.CreatePlayerInventory(chatId);
			await Map.CreatePlayerPosition(chatId);
			await BattleSystem.CreatePlayerBattle(chatId);
		}

		internal override bool IsFormattedCorrectly(string[] args)
		{
			if (args.Length == 1) return true;
			return false;
		}
	}
}

