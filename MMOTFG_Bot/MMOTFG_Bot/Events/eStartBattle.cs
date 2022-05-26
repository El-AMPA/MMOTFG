﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Events
{
    /// <summary>
    /// Starts a battle with a certain enemy
    /// </summary>
    class eStartBattle : Event
    {
        public string Enemy;
        public List<string> Enemies;

        public async override Task Execute(string chatId)
        {
            List<Enemy> enemies = new List<Enemy>();
            if (Enemy != null) enemies.Add(JSONSystem.GetEnemy(Enemy));
            else foreach (string s in Enemies) enemies.Add(JSONSystem.GetEnemy(s));

            bool isInParty = await PartySystem.IsInParty(chatId);
            bool isLeader = await PartySystem.IsLeader(chatId);

            if (isInParty && !isLeader) return;

            List<string> chatIds = new List<string>();

            chatIds.Add(chatId);
            if (isInParty)
            {
                string partyCode = await PartySystem.GetPartyCode(chatId);
                foreach (string id in await PartySystem.GetPartyMembers(partyCode))
                    chatIds.Add(id);
            }

            if (enemies.Count == 0) await TelegramCommunicator.SendText(chatId, "No valid enemies");

            else await BattleSystem.StartBattle(chatIds, enemies);
        }
    }
}
