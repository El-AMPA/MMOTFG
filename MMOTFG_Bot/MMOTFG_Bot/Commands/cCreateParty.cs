using System.Threading.Tasks;

namespace MMOTFG_Bot.Commands
{
	/// <summary>
	/// Shows the available directions from a given node.
	/// </summary>
	class cCreateParty : ICommand
	{
		public override void SetDescription()
		{
			commandDescription = @"Creates a new party. Can only be used if you are in a dungeon entrance.
Use: createParty [party code]. Party codes must be unique.";
		}
		public override void SetKeywords()
		{
			key_words = new string[] {
				"createparty"
			};
		}

		internal override async Task Execute(string command, string chatId, string[] args = null)
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

