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
			await PartySystem.CreateParty(partyCode, chatId);
		}

		internal override bool IsFormattedCorrectly(string[] args)
		{
			if (args.Length == 1) return true;
			return false;
		}
	}
}

