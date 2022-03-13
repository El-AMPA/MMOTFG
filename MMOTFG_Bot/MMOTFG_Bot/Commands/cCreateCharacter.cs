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
		public override void SetKeywords()
		{
			key_words = new string[] {
				"/create"
			};
		}

		async internal override void Execute(string command, long chatId, string[] args = null)
		{

			string charName = args[0];

			Dictionary<string, object> dict = new Dictionary<string, object>
			{

				{ DbConstants.PLAYER_FIELD_NAME , charName},
				{ DbConstants.PLAYER_FIELD_TELEGRAM_ID, chatId.ToString()}
			};

			bool created = await DatabaseManager.AddDocumentToCollection(dict, chatId.ToString(), DbConstants.COLLEC_DEBUG);

			if (!created)
			{
				await TelegramCommunicator.SendText(chatId, "That name is already in use");
				return;
			}

			await TelegramCommunicator.SendText(chatId, String.Format("Madre mia {0}, tremendo personaje acabas de crearte", charName));
			Console.WriteLine("Telegram user {0} just created characater with name {1}", chatId, charName);
		}

		internal override bool IsFormattedCorrectly(string[] args)
		{
			if (args.Length == 1) return true;
			return false;
		}
	}
}

