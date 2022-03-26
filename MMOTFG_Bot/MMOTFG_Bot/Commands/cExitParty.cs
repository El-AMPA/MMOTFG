using System.Threading.Tasks;

namespace MMOTFG_Bot.Commands
{
	/// <summary>
	/// Shows the available directions from a given node.
	/// </summary>
	class cExitParty : ICommand
	{
		public override void SetDescription()
		{
			commandDescription = @"Exits a party. Can only be used if you are in a party.
Use: exitParty. If you are the leader, the party will be deleted and all the members will exit the party as well.";
		}
		public override void SetKeywords()
		{
			key_words = new string[] {
				"exitparty"
			};
		}

		internal override async Task Execute(string command, long chatId, string[] args = null)
		{
			await PartySystem.ExitParty(chatId);
		}

		internal override bool IsFormattedCorrectly(string[] args)
		{
			if (args.Length == 0) return true;
			return false;
		}
	}
}

