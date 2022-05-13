﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MMOTFG_Bot.Battle;

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
            if (Enemy != null)
                await BattleSystem.StartBattle(chatId, JSONDeserializer.GetEnemy(Enemy));
            else
            {
                List<Battler> enemies = new List<Battler>();
                foreach (string s in Enemies) enemies.Add(JSONDeserializer.GetEnemy(s));
                await BattleSystem.StartBattle(chatId, enemies);
            }
        }
    }
}
