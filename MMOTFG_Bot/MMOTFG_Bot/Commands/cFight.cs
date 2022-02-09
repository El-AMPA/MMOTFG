﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    class cFight : ICommand
    {
        public override void Init()
        {
            key_words = new string[] {
                "/fight"
            };
        }

        internal override void Execute(string command, long chatId, string[] args = null)
        {
            //habría que preguntar al mapa qué enemigo hay en esta sala
            BattleSystem.startBattle(chatId, new Enemy());
            BattleSystem.setPlayerOptions(chatId);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /fight
            return true;
        }
    }
}