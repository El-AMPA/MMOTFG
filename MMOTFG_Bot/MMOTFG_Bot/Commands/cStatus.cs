﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Commands
{
    class cStatus : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Lists the player's stats.
use: stats";
        }
        public override void SetKeywords()
        {
            key_words = new string[] {
                "status",
                "s"
            };
        }

        internal override async Task Execute(string command, string chatId, string[] args = null)
        {
            if (args.Length == 0)
            {
                await BattleSystem.ShowStatus(chatId, BattleSystem.player);
                await InventorySystem.ShowGear(chatId);
            }
            else
            {
                //Chequeamos si es alguien de la party
                long? friendId = await PartySystem.GetFriendId(chatId, args[0]);
                if(friendId != null)
                {
                    await BattleSystem.ShowStatus(chatId, BattleSystem.player, friendId);
                    await InventorySystem.ShowGear(chatId, friendId);
                }
                //else await BattleSystem.ShowStatus(chatId, BattleSystem.enemy);
            }            
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /status enemy (optional)
            if (args.Length > 1) return false;

            return true;
        }
    }
}
