using System.Threading.Tasks;

namespace MMOTFG_Bot.Commands
{
	/// <summary>
	/// Shows the available directions from a given node.
	/// </summary>
	class cJoinParty : ICommand
	{
		public override void SetDescription()
		{
			commandDescription = @"Joins a party. Can only be used if you are in a dungeon entrance.
Use: joinParty [party code]. A party with that code must exist.";
		}
		public override void SetKeywords()
		{
			key_words = new string[] {
				"joinparty"
			};
		}

		internal override async Task Execute(string command, string chatId, string[] args = null)
		{
			string partyCode = args[0];
			await PartySystem.JoinParty(partyCode, chatId);
		}

		internal override bool IsFormattedCorrectly(string[] args)
		{
			if (args.Length == 1) return true;
			return false;
		}
	}
}

