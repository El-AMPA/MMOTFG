using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    static class PartySystem
    {
        public static async Task CreateParty(string code, long chatId)
        {
			bool partyExists = await PartyExists(code);
			if (partyExists)
			{
				await TelegramCommunicator.SendText(chatId, "A party with that code already exists!");
				return;
			}

			bool isInParty = await IsInParty(chatId);
			if (isInParty)
			{
				await TelegramCommunicator.SendText(chatId, "You are already in a party!");
				return;
			}

			Party party = new Party(code, chatId);
			bool created = await DatabaseManager.AddDocumentToCollection(party.getSerializable(), code, DbConstants.COLLEC_PARTIES);

			if (!created)
			{
				await TelegramCommunicator.SendText(chatId, "Error: Can't add the party to the database");
				Console.WriteLine("Error when trying to add party with partycode {} to the database", code);
				return;
			}

			await SetInParty(chatId, code);

			await TelegramCommunicator.SendText(chatId, String.Format("The party with code {0} has been created!", code));
			Console.WriteLine("Telegram user {0} just created party with name {1}", chatId, code);
		}
		
		public static async Task JoinParty(string code, long chatId)
        {
			bool partyExists = await PartyExists(code);
			if (!partyExists)
			{
				await TelegramCommunicator.SendText(chatId, "The party you tried to join does not exist!");
				return;
			}

			bool isInParty = await IsInParty(chatId);
			if (isInParty)
			{
				await TelegramCommunicator.SendText(chatId, "You are already in a party!");
				return;
			}

			Dictionary<string, object> party = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PARTY_FIELD_CODE, code, DbConstants.COLLEC_PARTIES);
			List<object> members = (List<object>)party[DbConstants.PARTY_FIELD_MEMBERS];
			members.Add(chatId);
			Dictionary<string, object> newMembersData = new Dictionary<string, object>
			{
				{ DbConstants.PARTY_FIELD_MEMBERS, members}
			};
			await DatabaseManager.ModifyDocumentFromCollection(newMembersData, code, DbConstants.COLLEC_PARTIES);
			await SetInParty(chatId, code);
			await TelegramCommunicator.SendText(chatId, "You have joined the party with code " + code);
		}

		public static async Task<bool> IsLeader(long chatId)
        {
			Dictionary<string, object>[] tempDict = await DatabaseManager.GetDocumentsByFieldValue(DbConstants.PARTY_FIELD_LEADER, chatId, DbConstants.COLLEC_PARTIES);
			return tempDict.GetLength(0) > 0;
        }

		static async Task<bool> PartyExists(string code)
        {
			Dictionary<string, object> tempDict = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PARTY_FIELD_CODE, code, DbConstants.COLLEC_PARTIES);
			return tempDict != null;
		}

		public static async Task<bool> IsInParty(long chatId)
        {
			Dictionary<string, object> tempDict = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID, chatId.ToString(), DbConstants.COLLEC_PLAYERS);
			return (bool)tempDict[DbConstants.PLAYER_ISINPARTY_FLAG];
		}

		public static async Task SetInParty(long chatId, string code)
        {
			Dictionary<string, object> partyInfo = new Dictionary<string, object> { 
				{ DbConstants.PLAYER_ISINPARTY_FLAG, true },
				{ DbConstants.PLAYER_PARTY_CODE, code }
			};
			await DatabaseManager.ModifyDocumentFromCollection(partyInfo, chatId.ToString(), DbConstants.COLLEC_PLAYERS);
        }
	}
}
