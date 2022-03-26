﻿using System;
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
Use: give [item name] [player name] [quantity]";
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

			int itemsGiven = 0;
			ObtainableItem item;

			if (InventorySystem.StringToItem(args[0], out item))
			{
				if (await InventorySystem.PlayerHasItem(chatId, item))
				{
					if (args.Length == 1) itemsGiven = await InventorySystem.ThrowAwayItem(chatId, item, 1);
					else
					{
						if (args[1] == "all") itemsGiven = await InventorySystem.ThrowAwayItem(chatId, item, -1);
						else itemsGiven = await InventorySystem.ThrowAwayItem(chatId, item, int.Parse(args[2]));
					}
					if (itemsGiven == 1) await TelegramCommunicator.SendText(chatId, "Item " + args[0] + " was sent to " + args[1] + ".");
					else if (itemsGiven > 1) await TelegramCommunicator.SendText(chatId, "Item " + args[0] + " was sent to " + args[1] + " " + itemsGiven + " times.");
				}
				else await TelegramCommunicator.SendText(chatId, "Item " + item.name + " couldn't be found in your inventory.");
			}
			else await TelegramCommunicator.SendText(chatId, "Item " + args[1] + " doesn't exist");

			//Your friend receives the item
			if(itemsGiven > 0)
            {
				await TelegramCommunicator.SendText((long)friendId, await PartySystem.GetPlayerName(chatId) + " has sent you something!");
				await InventorySystem.AddItem((long)friendId, item, itemsGiven);
			}
		}

		internal override bool IsFormattedCorrectly(string[] args)
		{
			if (args.Length == 2 || args.Length == 3) return true;
			return false;
		}
	}
}
