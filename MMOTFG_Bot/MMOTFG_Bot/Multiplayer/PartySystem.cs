using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace MMOTFG_Bot
{
    static class PartySystem
    {
        public static async Task CreateParty(string code, string chatId)
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

			await BattleSystem.CreatePartyBattleData(code);

			await SetPlayerInParty(chatId, code);

			await TelegramCommunicator.SendText(chatId, String.Format("The party with code {0} has been created!", code));
			Console.WriteLine("Telegram user {0} just created party with name {1}", chatId, code);
		}
		
		public static async Task JoinParty(string code, string chatId)
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

			if (await IsPartyBattling(code))
			{
				await TelegramCommunicator.SendText(chatId, "You can't enter the party in the middle of a battle!");
				return;
			}

			await AddPartyMember(code, chatId);
			await SetPlayerInParty(chatId, code);
			await TelegramCommunicator.SendText(chatId, "You have joined the party with code " + code);
			await TelegramCommunicator.SendText(chatId, await GetPlayerName(chatId) + " has joined the party!", true, chatId);
		}

		public static async Task<List<string>> GetPartyMembers(string code, bool includesLeader = false)
        {
			var party = await DatabaseManager.GetDocument(code, DbConstants.COLLEC_PARTIES);
			var partyIds = (List<object>)party[DbConstants.PARTY_FIELD_MEMBERS];
			if (includesLeader) partyIds.Add((string)party[DbConstants.PARTY_FIELD_LEADER]);
			return partyIds.Select(s=>(string)s).ToList();
		}

		static async Task AddPartyMember(string code, string chatId)
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

		static async Task RemovePartyMember(string code, string chatId)
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

		static async Task<bool> IsPartyBattling(string c)
        {
			if (!await PartyExists(c)) return false;
			return (bool)(await DatabaseManager.GetDocument(c, DbConstants.COLLEC_PARTIES))[DbConstants.BATTLE_ACTIVE];
        }

		public static async Task ExitParty(string chatId)
        {
			bool isInParty = await IsInParty(chatId);
			if (!isInParty)
			{
				await TelegramCommunicator.SendText(chatId, "You are not in a party!");
				return;
			}

			string code = await GetPartyCode(chatId);

			if (await IsPartyBattling(code))
            {
				await TelegramCommunicator.SendText(chatId, "You can't exit the party in the middle of a battle!");
				return;
            }

			string playerName = await GetPlayerName(chatId);
			bool isLeader = await IsLeader(chatId);

			var battlerInfo = (Dictionary<string, object>)(await DatabaseManager.GetDocument(code, DbConstants.COLLEC_PARTIES))[DbConstants.PARTY_FIELD_MEMBER_INFO];
			await DatabaseManager.ModifyFieldOfDocument(DbConstants.PLAYER_FIELD_BATTLE_INFO, (Dictionary<string, object>)battlerInfo[chatId], chatId, DbConstants.COLLEC_PLAYERS);

			if (!isLeader)
            {
				await TelegramCommunicator.SendText(chatId, playerName + " has left the party.", true, chatId);
				await RemovePartyMember(code, chatId);
				await SetPlayerOutOfParty(chatId);
				await TelegramCommunicator.SendText(chatId, "You have left the party.");
            }
            else
            {
				await TelegramCommunicator.SendText(chatId, "The leader exited the party. The party has been deleted", true);
				await DeleteParty(code);
				await TelegramCommunicator.SendText(chatId, "Your party has been deleted");
            }
		}

		static async Task DeleteParty(string code)
        {
			var party = await DatabaseManager.GetDocument(code, DbConstants.COLLEC_PARTIES);

			List<object> members = (List<object>)party[DbConstants.PARTY_FIELD_MEMBERS];
			members.Add(party[DbConstants.PARTY_FIELD_LEADER]);

			foreach (string id in members) await SetPlayerOutOfParty(id);

			await DatabaseManager.DeleteDocumentById(code, DbConstants.COLLEC_PARTIES);
		}

		public static async Task ShowParty(string chatId)
        {
			if(!await IsInParty(chatId))
            {
				await TelegramCommunicator.SendText(chatId, "You are not in a party!");
				return;
            }

			var party = await DatabaseManager.GetDocument(await GetPartyCode(chatId), DbConstants.COLLEC_PARTIES);
			string partyInfo = "PARTY " + (string)party[DbConstants.PARTY_FIELD_CODE] + '\n';
			partyInfo += "LEADER: " + await GetPlayerName((string)party[DbConstants.PARTY_FIELD_LEADER]) + '\n' + "MEMBERS: \n";
			foreach(string id in (List<object>)party[DbConstants.PARTY_FIELD_MEMBERS])
				partyInfo += await GetPlayerName(id) + '\n';

			await TelegramCommunicator.SendText(chatId, partyInfo);
		}

		//public static async Task<string> GetLeaderId(string code)
  //      {
		//	return (string)(await DatabaseManager.GetDocument(code, DbConstants.COLLEC_PARTIES))[DbConstants.PARTY_FIELD_LEADER];
  //      }

		/// <summary>
		/// Returns the code of the player's party.
		/// </summary>
		/// <param name="chatId"></param>
		/// <returns></returns>
		public static async Task<string> GetPartyCode(string chatId)
        {
			if (!(await IsInParty(chatId))) return null;
			var player = await DatabaseManager.GetDocument(chatId, DbConstants.COLLEC_PLAYERS);
			return (string)player[DbConstants.PLAYER_PARTY_CODE];
        }

		/// <summary>
		/// Returns true if the player is the leader of their party.
		/// </summary>
		/// <param name="chatId"></param>
		/// <returns></returns>
		public static async Task<bool> IsLeader(string chatId)
        {
			bool isInParty = await IsInParty(chatId);
			if (!isInParty) return false;
			string code = await GetPartyCode(chatId);

			var party = await DatabaseManager.GetDocument(code, DbConstants.COLLEC_PARTIES);
			return chatId == (string)party[DbConstants.PARTY_FIELD_LEADER];
        }

		/// <summary>
		/// Returns the given Id's player name.
		/// </summary>
		/// <param name="chatId"></param>
		/// <returns></returns>
		public static async Task<string> GetPlayerName(string chatId)
        {
			var player = await DatabaseManager.GetDocument(chatId, DbConstants.COLLEC_PLAYERS);
			return (string)player[DbConstants.PLAYER_FIELD_NAME];
        }

		public static async Task<string> GetFriendId(string chatId, string name)
        {
			var player = await DatabaseManager.GetDocument(chatId, DbConstants.COLLEC_PLAYERS);
			if (!await IsInParty(chatId)) return null;

			string partyCode = (string)player[DbConstants.PLAYER_PARTY_CODE];
			var party = await DatabaseManager.GetDocument(partyCode, DbConstants.COLLEC_PARTIES);
			if(await PartyExists(partyCode)){
				string aux = (string)party[DbConstants.PARTY_FIELD_LEADER];
				if (name == await GetPlayerName(aux)) return aux;

				List<object> members = (List<object>)party[DbConstants.PARTY_FIELD_MEMBERS];
				foreach(string id in members)
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
		public static async Task<bool> IsInParty(string chatId)
        {
			var player = await DatabaseManager.GetDocument(chatId, DbConstants.COLLEC_PLAYERS);
			return (bool)player[DbConstants.PLAYER_ISINPARTY_FLAG];
		}

		public static async Task<bool> IsInParty(string playerName, string partyCode)
        {
			var player = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_NAME, playerName, DbConstants.COLLEC_PLAYERS);
			return player != null && (bool)player[DbConstants.PLAYER_ISINPARTY_FLAG] && ((string)player[DbConstants.PLAYER_PARTY_CODE] == partyCode);
		}

		/// <summary>
		/// Registers that the gicen player is in a party in the database.
		/// </summary>
		/// <param name="chatId">Id of the player</param>
		/// <param name="code">Code of the party</param>
		/// <returns></returns>
		static async Task SetPlayerInParty(string chatId, string code)
        {
			Dictionary<string, object> partyInfo = new Dictionary<string, object> { 
				{ DbConstants.PLAYER_ISINPARTY_FLAG, true },
				{ DbConstants.PLAYER_PARTY_CODE, code }
			};
			await DatabaseManager.ModifyDocumentFromCollection(partyInfo, chatId, DbConstants.COLLEC_PLAYERS);
        }

		static async Task SetPlayerOutOfParty(string chatId)
        {
			Dictionary<string, object> partyInfo = new Dictionary<string, object> {
				{ DbConstants.PLAYER_ISINPARTY_FLAG, false },
				{ DbConstants.PLAYER_PARTY_CODE, null }
			};
			await DatabaseManager.ModifyDocumentFromCollection(partyInfo, chatId, DbConstants.COLLEC_PLAYERS);
		}

		public static async Task<bool> IsPartyWipedOut(string code)
        {
			return (bool)(await DatabaseManager.GetDocument(code, DbConstants.COLLEC_PARTIES))[DbConstants.PARTY_FIELD_WIPEOUT];
        }

		public static async Task WipeOutParty(string code, bool wipeout)
        {
            Dictionary<string, object> partyInfo = new Dictionary<string, object> {
                { DbConstants.PARTY_FIELD_WIPEOUT, wipeout }
            };
            await DatabaseManager.ModifyDocumentFromCollection(partyInfo, code, DbConstants.COLLEC_PARTIES);
        }
    }
}
