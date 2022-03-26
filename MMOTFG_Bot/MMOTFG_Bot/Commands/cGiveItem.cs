using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Commands
{
	/// <summary>
	/// Uses an item a specified amount of times.
	/// The user can add 'all' to consume ALL instances of that item.
	/// </summary>
	class cGiveItem : ICommand
	{
		public override void SetDescription()
		{
			commandDescription = @"Gives an item on your inventory to a fellow party member.
Use: give [item name] [player name]";
		}
		public override void SetKeywords()
		{
			key_words = new string[]{
				"give"
			};
		}

		internal override async Task Execute(string command, long chatId, string[] args = null)
		{
			//if(args.Length == 1) await InventorySystem.ConsumeItem(chatId, args[0], 1, command, args);
			//else
			//{
			//	if(args[1] == "all") await InventorySystem.ConsumeItem(chatId, args[0], -1, command, args);
			//	else await InventorySystem.ConsumeItem(chatId, args[0], int.Parse(args[1]), command, args);
			//}


			if(!await PartySystem.IsInParty(chatId))
			{
				await TelegramCommunicator.SendText(chatId, "You can't give this to anyone!");
				return;
			}

			long? friendId = await PartySystem.GetFriendId(chatId, args[1]);
			if(friendId == null)
			{
				await TelegramCommunicator.SendText(chatId, "That player is not in your party!");
				return;
			}
			//You lose the item


			//Your friend receives the item
			await TelegramCommunicator.SendText((long)friendId, await PartySystem.GetPlayerName(chatId) + " has sent you something!");
			await InventorySystem.AddItem((long)friendId, args[0], 1);
		}

		internal override bool IsFormattedCorrectly(string[] args)
		{
			if (args.Length == 2) return true;
			return false;
		}
	}
}
