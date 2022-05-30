using System.Threading.Tasks;
using MMOTFG_Bot.Multiplayer;

namespace MMOTFG_Bot.Communicator
{
	/// <summary>
	/// Shows the available directions from a given node.
	/// </summary>
	class cShowParty : ICommand
	{
		public override void SetDescription()
		{
			commandDescription = @"Shows the members of your current party.
Use: showParty. You must be in a party.";
		}
		public override void SetKeywords()
		{
			key_words = new string[] {
				"showparty", "party"
			};
		}

		internal override async Task Execute(string command, string chatId, string[] args = null)
		{
			await PartySystem.ShowParty(chatId);
		}

		internal override bool IsFormattedCorrectly(string[] args)
		{
			if (args.Length == 0) return true;
			return false;
		}
	}
}

