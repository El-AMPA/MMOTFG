using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MMOTFG_Bot.Navigation;

namespace MMOTFG_Bot.Commands
{
	/// <summary>
	/// Sends the 'OnInspectText' field of the current node of the player 
	/// </summary>
	class cInspectRoom : ICommand
	{
		public override void SetDescription()
		{
			commandDescription = @"Inspects the current room
Use: inpect";
		}
		public override void SetKeywords()
		{
			key_words = new string[] {
				"inspect",
				"look",
				"ins",
				"i"
			};
		}

		internal override async Task Execute(string command, long chatId, string[] args = null)
		{
			bool inParty = await PartySystem.IsInParty(chatId);
			if(!inParty) await Map.OnInspect(chatId);
			else
			{
				bool leader = await PartySystem.IsLeader(chatId);
				if (!leader) await TelegramCommunicator.SendText(chatId, "Only the party leader can inspect a room.");
                else
                {
					string code = await PartySystem.GetPartyCode(chatId);
					await Map.OnInspect(chatId, code);
                }
			}
		}

		internal override bool IsFormattedCorrectly(string[] args)
		{
			if (args.Length == 0) return true;
			return false;
		}
	}
}
