﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{/// <summary>
 /// Shows the available directions from a given node.
 /// </summary>
	class cDebug : ICommand
	{
		public override void SetKeywords()
		{
			key_words = new string[] {
				"/debug"
			};
		}

		async internal override void Execute(string command, long chatId, string[] args = null)
		{			

			string arg0 = args[0];

			switch (arg0.ToLower())
			{
				case "mapsave":
					await Navigation.Map.SavePlayerPosition(chatId);
					Console.WriteLine("--- using debug command {0} ---", arg0);
					break;
				case "mapload":
					await Navigation.Map.LoadPlayerPosition(chatId);
					Console.WriteLine("--- using debug command {0} ---", arg0);
					break;
				case "resetinv":
					await InventorySystem.CreatePlayerInventory(chatId);
					Console.WriteLine("--- using debug command {0} ---", arg0);
					break;
			}
				

		}

		internal override bool IsFormattedCorrectly(string[] args)
		{
			if (args.Length > 0) return true;
			return false;
		}
	}
}
