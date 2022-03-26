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

			await SetPlayerInParty(chatId, code);

			await TelegramCommunicator.SendText(chatId, String.Format("The party with code {0} has been created!", code));
			Console.WriteLine("Telegram user {0} just created party with name {1}", chatId, code);
		}
		
		public static async Task JoinParty(string code, long chatId)
        {
			bool isInParty = await IsInParty(chatId);
			if (isInParty)
			{
				await TelegramCommunicator.SendText(chatId, "You are already in a party!");
				return;
			}

			bool partyExists = await PartyExists(code);
			if (!partyExists)
			{
				await TelegramCommunicator.SendText(chatId, "The party you tried to join does not exist!");
				return;
			}

			await AddPartyMember(code, chatId);
			await SetPlayerInParty(chatId, code);
			await TelegramCommunicator.SendText(chatId, "You have joined the party with code " + code);
			string name = await GetPlayerName(chatId);
			await BroadcastMessage(name + " has joined the party!", code, chatId);
		}

		public static async Task<List<object>> GetPartyMembers(string code)
        {
			var party = await DatabaseManager.GetDocument(code, DbConstants.COLLEC_PARTIES);
			return (List<object>)party[DbConstants.PARTY_FIELD_MEMBERS];
		}

		static async Task AddPartyMember(string code, long chatId)
        {
			var party = await DatabaseManager.GetDocument(code, DbConstants.COLLEC_PARTIES);
			List<object> members = (List<object>)party[DbConstants.PARTY_FIELD_MEMBERS];
			members.Add(chatId);
			Dictionary<string, object> newMembersData = new Dictionary<string, object>
			{
				{ DbConstants.PARTY_FIELD_MEMBERS, members}
			};
			await DatabaseManager.ModifyDocumentFromCollection(newMembersData, code, DbConstants.COLLEC_PARTIES);
		}

		static async Task RemovePartyMember(string code, long chatId)
        {
			var party = await DatabaseManager.GetDocument(code, DbConstants.COLLEC_PARTIES);
			List<object> members = (List<object>)party[DbConstants.PARTY_FIELD_MEMBERS];
			members.Remove(chatId);
			Dictionary<string, object> newMembersData = new Dictionary<string, object>
			{
				{ DbConstants.PARTY_FIELD_MEMBERS, members}
			};
			await DatabaseManager.ModifyDocumentFromCollection(newMembersData, code, DbConstants.COLLEC_PARTIES);
		}

		public static async Task ExitParty(long chatId)
        {
			bool isInParty = await IsInParty(chatId);
			if (!isInParty)
			{
				await TelegramCommunicator.SendText(chatId, "You are not in a party!");
				return;
			}

			string code = await GetPartyCode(chatId);
			string playerName = await GetPlayerName(chatId);
			bool isLeader = await IsLeader(chatId);

            if (!isLeader)
            {
				await RemovePartyMember(code, chatId);
				await BroadcastMessage(playerName + " has left the party.", code);
				await SetPlayerOutOfParty(chatId);
				await TelegramCommunicator.SendText(chatId, "You have left the party.");
            }
            else
            {
				await BroadcastMessage("The leader exited the party. The party has been deleted", code, chatId);
				await DeleteParty(code);
				await TelegramCommunicator.SendText(chatId, "Your party has been deleted");
				
            }
		}

		static async Task DeleteParty(string code)
        {
			var party = await DatabaseManager.GetDocument(code, DbConstants.COLLEC_PARTIES);

			List<object> members = (List<object>)party[DbConstants.PARTY_FIELD_MEMBERS];
			members.Add(party[DbConstants.PARTY_FIELD_LEADER]);

			foreach (long id in members) await SetPlayerOutOfParty(id);

			await DatabaseManager.DeleteDocumentById(code, DbConstants.COLLEC_PARTIES);
		}

		public static async Task ShowParty(long chatId)
        {
			if(!await IsInParty(chatId))
            {
				await TelegramCommunicator.SendText(chatId, "You are not in a party!");
				return;
            }

			var party = await DatabaseManager.GetDocument(await GetPartyCode(chatId), DbConstants.COLLEC_PARTIES);
			string partyInfo = "PARTY " + (string)party[DbConstants.PARTY_FIELD_CODE] + '\n';
			partyInfo += "LEADER: " + await GetPlayerName((long)party[DbConstants.PARTY_FIELD_LEADER]) + '\n' + "MEMBERS: \n";
			foreach(long id in (List<object>)party[DbConstants.PARTY_FIELD_MEMBERS])
				partyInfo += await GetPlayerName(id) + '\n';

			await TelegramCommunicator.SendText(chatId, partyInfo);
		}

		/// <summary>
		/// Returns the code of the player's party.
		/// </summary>
		/// <param name="chatId"></param>
		/// <returns></returns>
		public static async Task<string> GetPartyCode(long chatId)
        {
			var player = await DatabaseManager.GetDocument(chatId.ToString(), DbConstants.COLLEC_PLAYERS);
			return (string)player[DbConstants.PLAYER_PARTY_CODE];
        }

		/// <summary>
		/// Returns true if the player is the leader of their party.
		/// </summary>
		/// <param name="chatId"></param>
		/// <returns></returns>
		public static async Task<bool> IsLeader(long chatId)
        {
			bool isInParty = await IsInParty(chatId);
			string code = await GetPartyCode(chatId);

			var party = await DatabaseManager.GetDocument(code, DbConstants.COLLEC_PARTIES);
			return chatId == (long)party[DbConstants.PARTY_FIELD_LEADER];
        }

		/// <summary>
		/// Returns the given Id's player name.
		/// </summary>
		/// <param name="chatId"></param>
		/// <returns></returns>
		public static async Task<string> GetPlayerName(long chatId)
        {
			var player = await DatabaseManager.GetDocument(chatId.ToString(), DbConstants.COLLEC_PLAYERS);
			return (string)player[DbConstants.PLAYER_FIELD_NAME];
        }

		public static async Task<long?> GetFriendId(long chatId, string name)
        {
			var player = await DatabaseManager.GetDocument(chatId.ToString(), DbConstants.COLLEC_PLAYERS);
			if (!await IsInParty(chatId)) return null;

			string partyCode = (string)player[DbConstants.PLAYER_PARTY_CODE];
			var party = await DatabaseManager.GetDocument(partyCode, DbConstants.COLLEC_PARTIES);
			if(await PartyExists(partyCode)){
				long aux = (long)party[DbConstants.PARTY_FIELD_LEADER];
				if (name == await GetPlayerName(aux)) return aux;

				List<object> members = (List<object>)party[DbConstants.PARTY_FIELD_MEMBERS];
				foreach(long id in members)
					if (await GetPlayerName(id) == name) return id;	
            }
			return null;
        }

		/// <summary>
		/// Returns true if the party exists, false otherwise.
		/// </summary>
		/// <param name="code"></param>
		/// <returns></returns>
		static async Task<bool> PartyExists(string code)
        {
			var party = await DatabaseManager.GetDocument(code, DbConstants.COLLEC_PARTIES);
			return party != null;
		}

		/// <summary>
		/// Returns true if the player is in a party, false otherwise.
		/// </summary>
		/// <param name="chatId"></param>
		/// <returns></returns>
		public static async Task<bool> IsInParty(long chatId)
        {
			var player = await DatabaseManager.GetDocument(chatId.ToString(), DbConstants.COLLEC_PLAYERS);
			return (bool)player[DbConstants.PLAYER_ISINPARTY_FLAG];
		}

		/// <summary>
		/// Registers that the gicen player is in a party in the database.
		/// </summary>
		/// <param name="chatId">Id of the player</param>
		/// <param name="code">Code of the party</param>
		/// <returns></returns>
		static async Task SetPlayerInParty(long chatId, string code)
        {
			Dictionary<string, object> partyInfo = new Dictionary<string, object> { 
				{ DbConstants.PLAYER_ISINPARTY_FLAG, true },
				{ DbConstants.PLAYER_PARTY_CODE, code }
			};
			await DatabaseManager.ModifyDocumentFromCollection(partyInfo, chatId.ToString(), DbConstants.COLLEC_PLAYERS);
        }

		static async Task SetPlayerOutOfParty(long chatId)
        {
			Dictionary<string, object> partyInfo = new Dictionary<string, object> {
				{ DbConstants.PLAYER_ISINPARTY_FLAG, false },
				{ DbConstants.PLAYER_PARTY_CODE, null }
			};
			await DatabaseManager.ModifyDocumentFromCollection(partyInfo, chatId.ToString(), DbConstants.COLLEC_PLAYERS);
		}

		/// <summary>
		/// Sends a message to everyone in the party. If a chatId is specified, the method will not send the message to said user.
		/// </summary>
		/// <param name="message">Message to be broadcasted</param>
		/// <param name="code">Code of the party</param>
		/// <param name="chatId">Id of the sender</param>
		/// <returns></returns>
		public static async Task BroadcastMessage(string message, string code, long chatId = 0)
        {
			var party = await DatabaseManager.GetDocument(code, DbConstants.COLLEC_PARTIES);

			List<object> members = (List<object>)party[DbConstants.PARTY_FIELD_MEMBERS];
			members.Add(party[DbConstants.PARTY_FIELD_LEADER]);

			foreach(long id in members)
            {
				if (id != chatId) await TelegramCommunicator.SendText(id, message);
            }
		}
	}
}
