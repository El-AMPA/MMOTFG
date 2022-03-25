using System;
using System.Collections.Generic;
using System.Text;
using MMOTFG_Bot.Navigation;

namespace MMOTFG_Bot.Commands
{
	/// <summary>
	/// Shows the available directions from a given node.
	/// </summary>
	class cCreateParty : ICommand
	{
		public override void setDescription()
		{
			commandDescription = @"Creates a new party. Can only be used if you are in a dungeon entrance.
Use: create [party code]. Party codes must be unique.";
		}
		public override void SetKeywords()
		{
			key_words = new string[] {
				"createparty"
			};
		}

		async internal override void Execute(string command, long chatId, string[] args = null)
		{

			string partyCode = args[0];

			Dictionary<string,object>tempDict = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PARTY_FIELD_CODE, partyCode, DbConstants.COLLEC_PARTIES);

			if(tempDict != null)
			{
				await TelegramCommunicator.SendText(chatId, "A party with that code already exists!");
				return;
			}

			tempDict = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID, chatId.ToString(), DbConstants.COLLEC_PLAYERS);

			if ((bool)tempDict[DbConstants.PLAYER_ISINPARTY_FLAG])
			{
				await TelegramCommunicator.SendText(chatId, "You are already in a party!");
				return;
			}

			List<long> partyMembers = new List<long>();
			partyMembers.Add(chatId);

			Dictionary<string, object> dict = new Dictionary<string, object>
			{
				{ DbConstants.PARTY_FIELD_CODE , partyCode},
				{ DbConstants.PARTY_FIELD_MEMBERS, partyMembers }
			};

			bool created = await DatabaseManager.AddDocumentToCollection(dict, chatId.ToString(), DbConstants.COLLEC_PARTIES);

			if (!created)
			{
				await TelegramCommunicator.SendText(chatId, "Error: Can't add the party to the database");
				Console.WriteLine("Error when trying to add party with partycode {} to the database", partyCode);
				return;
			}

			await TelegramCommunicator.SendText(chatId, String.Format("The party with code {0} has been created!", partyCode));
			Console.WriteLine("Telegram user {0} just created party with name {1}", chatId, partyCode);

			//await InventorySystem.CreatePlayerInventory(chatId);
			//await Map.CreatePlayerPosition(chatId);
			//await BattleSystem.CreatePlayerBattle(chatId);
		}

		internal override bool IsFormattedCorrectly(string[] args)
		{
			if (args.Length == 1) return true;
			return false;
		}
	}
}

