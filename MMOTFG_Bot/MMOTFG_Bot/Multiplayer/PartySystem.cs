using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    static class PartySystem
    {
        public static async Task CreateParty(string code, long leaderId)
        {
			Dictionary<string, object> tempDict = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PARTY_FIELD_CODE, code, DbConstants.COLLEC_PARTIES);

			if (tempDict != null)
			{
				await TelegramCommunicator.SendText(leaderId, "A party with that code already exists!");
				return;
			}

			tempDict = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID, leaderId.ToString(), DbConstants.COLLEC_PLAYERS);

			if ((bool)tempDict[DbConstants.PLAYER_ISINPARTY_FLAG])
			{
				await TelegramCommunicator.SendText(leaderId, "You are already in a party!");
				return;
			}

			Party party = new Party(code, leaderId);
			bool created = await DatabaseManager.AddDocumentToCollection(party.getSerializable(), code, DbConstants.COLLEC_PARTIES);

			if (!created)
			{
				await TelegramCommunicator.SendText(leaderId, "Error: Can't add the party to the database");
				Console.WriteLine("Error when trying to add party with partycode {} to the database", code);
				return;
			}

			await SetInParty(leaderId, code);

			await TelegramCommunicator.SendText(leaderId, String.Format("The party with code {0} has been created!", code));
			Console.WriteLine("Telegram user {0} just created party with name {1}", leaderId, code);
		}

		public static async Task IsLeader(long chatId)
        {

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
