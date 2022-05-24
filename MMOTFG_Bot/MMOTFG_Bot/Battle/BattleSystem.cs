﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MMOTFG_Bot.StatName;

namespace MMOTFG_Bot
{
    static class BattleSystem
    {
        public static List<Player> players;

        public static List<Battler> enemies;

        public static List<Battler> battlers;

        public static bool battleActive = false;
        public static bool battlePaused = false;

        private static string partyCode = null;

        public static void Init()
        {
        }

        public static Player GetPlayer(string chatId)
        {
            if (partyCode == null)
                return players.First();
            else
            {
                return players.FirstOrDefault(p => p.id == chatId);
            }
        }

        public static async Task<Player> CreatePlayer(string chatId)
        {
            Player player = JSONSystem.GetDefaultPlayer();
            var info = await DatabaseManager.GetFieldFromDocument(DbConstants.PLAYER_FIELD_BATTLE_INFO, chatId, DbConstants.COLLEC_PLAYERS);
            if (info != null)
                player.LoadSerializable((Dictionary<string, object>)info);
            return player;
        }

        public static async Task SavePlayerBattle(string chatId)
        {     
            if (partyCode == null)
            {
                //Save player info in the player field of the database
                await DatabaseManager.ModifyFieldOfDocument(DbConstants.PLAYER_FIELD_BATTLE_INFO, players.First().GetSerializable(), chatId, DbConstants.COLLEC_PLAYERS);
            }
            else
            {               
                //Save players of the party 
                Dictionary<string, object> playersDict = new Dictionary<string, object>();
                foreach(Player p in players)
                {
                    playersDict.Add(p.id, p.GetSerializable());
                }
                await DatabaseManager.ModifyFieldOfDocument(DbConstants.PARTY_FIELD_MEMBER_INFO, playersDict, partyCode, DbConstants.COLLEC_PARTIES);
            }

            //Data of the battle (enemies, battle state, etc)
            Dictionary<string, object> update = new Dictionary<string, object>();
            update.Add(DbConstants.BATTLE_ACTIVE, battleActive);
            update.Add(DbConstants.BATTLE_PAUSED, battlePaused);
            if (!battleActive)
            {
                update.Add(DbConstants.BATTLE_ENEMY_LIST, null);
            }
            else
            {
                List<Dictionary<string, object>> enemiesDict = new List<Dictionary<string, object>>();
                foreach (Battler e in enemies)
                {
                    enemiesDict.Add(e.GetSerializable());
                }
                update.Add(DbConstants.BATTLE_ENEMY_LIST, enemiesDict);
            }

            //If the player is in a party the data is stored in the party database field (or in the player field if this is not the case)
            if(partyCode != null)
                await DatabaseManager.ModifyDocumentFromCollection(update, partyCode, DbConstants.COLLEC_PARTIES);
            else await DatabaseManager.ModifyDocumentFromCollection(update, chatId, DbConstants.COLLEC_PLAYERS);
        }

        public static async Task LoadPlayerBattle(string chatId)
        {
            Dictionary<string, object> dbInfo = new Dictionary<string, object>();

            partyCode = await PartySystem.GetPartyCode(chatId);

            if (partyCode == null)
            {
                dbInfo = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID,
                chatId, DbConstants.COLLEC_PLAYERS);
                Player p = JSONSystem.GetDefaultPlayer();
                p.LoadSerializable((Dictionary<string, object>)dbInfo[DbConstants.PLAYER_FIELD_BATTLE_INFO]);
                p.SetId(chatId);
                players = new List<Player>();
                players.Add(p);
            }
            else
            {
                dbInfo = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PARTY_FIELD_CODE, partyCode, DbConstants.COLLEC_PARTIES);
                Dictionary<string, object> dbPlayers = (Dictionary<string, object>)dbInfo[DbConstants.PARTY_FIELD_MEMBER_INFO];
                players = new List<Player>();
                foreach (var key in dbPlayers.Keys)
                {
                    Player p = JSONSystem.GetDefaultPlayer();
                    p.LoadSerializable((Dictionary<string, object>)dbPlayers[key]);
                    p.SetId(key);
                    players.Add(p);
                }
            }

            battleActive = (bool)dbInfo[DbConstants.BATTLE_ACTIVE];
         
            if (!battleActive) return;

            battlePaused = (bool)dbInfo[DbConstants.BATTLE_PAUSED];
          
            List<object> enemiesDict = (List<object>)dbInfo[DbConstants.BATTLE_ENEMY_LIST];

            enemies = new List<Battler>();

            foreach (Dictionary<string, object> o in enemiesDict)
            {
                Battler b = JSONSystem.GetEnemy((string)o[DbConstants.BATTLER_FIELD_NAME]);
                b.LoadSerializable(o);
                enemies.Add(b);
            }

            battlers = new List<Battler>();
            battlers.AddRange(enemies);
            battlers.AddRange(players);
        }

        public static async Task CreatePlayerBattleData(string chatId)
        {
            Dictionary<string, object> update = new Dictionary<string, object>();

            //Creamos un jugador nuevo con la info del json
            Player player = JSONSystem.GetDefaultPlayer();
            player.SetName((string)(await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID, chatId, DbConstants.COLLEC_PLAYERS))[DbConstants.PLAYER_FIELD_NAME]);
            update.Add(DbConstants.BATTLE_ACTIVE, false);
            update.Add(DbConstants.BATTLE_PAUSED, false);
            update.Add(DbConstants.PLAYER_FIELD_BATTLE_INFO, player.GetSerializable());
            update.Add(DbConstants.BATTLE_ENEMY_LIST, null);
            players = new List<Player>() { player };

            await DatabaseManager.ModifyDocumentFromCollection(update, chatId, DbConstants.COLLEC_PLAYERS);
        }

        public static async Task CreatePartyBattleData(string partyCode)
        {
            Dictionary<string, object> update = new Dictionary<string, object>();

            update.Add(DbConstants.BATTLE_ACTIVE, false);
            update.Add(DbConstants.BATTLE_PAUSED, false);
            //Save players of the party 
            Dictionary<string, object> playersDict = new Dictionary<string, object>();
            List<string> ids = await PartySystem.GetPartyMembers(partyCode, true);
            players = new List<Player>();
            foreach (string id in ids)
            {
                Player player = await CreatePlayer(id);
                player.SetName((string)(await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID, id, DbConstants.COLLEC_PLAYERS))[DbConstants.PLAYER_FIELD_NAME]);
                player.SetId(id);
                playersDict.Add(id, player.GetSerializable());
                players.Add(player);
            }
            update.Add(DbConstants.PARTY_FIELD_MEMBER_INFO, playersDict);
            update.Add(DbConstants.BATTLE_ENEMY_LIST, null);

            await DatabaseManager.ModifyDocumentFromCollection(update, partyCode, DbConstants.COLLEC_PARTIES);
        }

        public static async Task<bool> IsPlayerInBattle(string chatId)
        {
            return (bool)await DatabaseManager.GetFieldFromDocument(DbConstants.BATTLE_ACTIVE, chatId, DbConstants.COLLEC_PLAYERS);
        }

        public static async Task StartBattle(string chatId, Battler eSide)
        {
            await StartBattle(new List<string>() { chatId }, new List<Battler> { eSide });
        }

        public static async Task StartBattle(List<string> chatIds, List<Battler> eSide)
        {
            await LoadPlayerBattle(chatIds.First());
            enemies = eSide;
            battlers = new List<Battler>();
            battlers.AddRange(players);
            battlers.AddRange(enemies);
            battleActive = true;
            battlePaused = false;
            if(eSide.Count == 1)
            {
                Battler b = eSide.First();
                if (b.imageName != null)
                {
                    await TelegramCommunicator.SendImage(chatIds.First(), b.imageName, true, b.imageCaption);
                }
            }
            else
            {
                List<string> imageNames = new List<string>();
                string caption = "";
                foreach (Battler b in eSide)
                {
                    if (b.imageName != null)
                        imageNames.Add(b.imageName);

                    if (b.imageCaption != null)
                        caption += b.imageCaption + "\n";
                }

                //if multiple enemies have the same name, they need to be distinct
                List<string> repeatedNames = eSide.GroupBy(e => e.name).Where(g => g.Count() > 1).Select(y => y.Key).ToList();
                foreach(string name in repeatedNames)
                {
                    List<Battler> enemiesWithName = eSide.Where(e => e.name == name).ToList();
                    for(int i = 0; i < enemiesWithName.Count; i++)
                    {
                        //this way, you get Enemy_1, Enemy_2...
                        enemiesWithName[i].name += $"_{i + 1}";
                    }
                }

                await TelegramCommunicator.SendImageCollection(chatIds.First(), imageNames.ToArray(), true);
                await TelegramCommunicator.SendText(chatIds.First(), caption, true);
            }
            //all battlers start being able to move
            foreach (Battler ba in battlers)
            {
                ba.turnOver = false;
            }
            //when there are multiple players, enemies get stronger
            if (players.Count > 1)
                foreach (Battler b in enemies)
                    b.BoostStats(players.Count);
            await SavePlayerBattle(chatIds.First());
            await NextAttack(chatIds.First());
        }

        public static async Task NextAttack(string chatId)
        {
            if (!battleActive || battlePaused) return;

            //if every battler has finished their turn, start a new turn
            if (battlers.FirstOrDefault(x => x.turnOver == false) == null)
            {
                foreach (Battler ba in battlers)
                {
                    if (ba.GetStat(HP) > 0)
                    {
                        await ba.OnBehaviour(chatId, ba.onTurnEnd);
                        ba.turnOver = false;
                    }   
                }
            }
            //sort battlers by speed
            battlers.Sort((b1, b2) => b2.GetStat(SPE).CompareTo(b1.GetStat(SPE)));
            //get first battler that hasn't moved that turn and is alive
            Battler b = battlers.First(x => x.turnOver == false);

            string message = $"{b.name}'s turn";
            await TelegramCommunicator.SendText(chatId, message, true);
            //if the battler is an enemy
            if (b as Player == null)
            {
                Attack a = b.NextAttack();
                Battler target = b;
                if (!a.affectsSelf)
                {
                    List<Player> alivePlayers = players.Where(x => x.GetStat(HP) > 0).ToList();
                    target = alivePlayers[RNG.Next(0, alivePlayers.Count)];
                }
                await SavePlayerBattle(chatId);
                await UseAttack(chatId, a, b, target);
            }
            //if the battler is a player
            else
            {
                string id = chatId;
                if (partyCode != null)
                    id = await PartySystem.GetFriendId(chatId, b.name);
                await SetPlayerOptions(id, "Select an attack");
            }
        }

        public static async Task SetPlayerOptions(string chatId, string text)
        {
            Player player = GetPlayer(chatId);
            player.upNext = true;
            await SavePlayerBattle(chatId);
            await TelegramCommunicator.SendButtons(chatId, text, player.attacks);
        }

        public static async Task PlayerAttack(string chatId, string attackName, string targetName = null)
        {
            Player player = GetPlayer(chatId);
            if (!battleActive)
            {
                await TelegramCommunicator.SendText(chatId, "No battle currently active");
                return;
            }
            if (!player.upNext)
            {
                await TelegramCommunicator.SendText(chatId, "Not your turn");
                return;
            }
            Attack attack = player.GetAttack(attackName);
            if (attack == null)
            {
                await TelegramCommunicator.SendText(chatId, "Invalid attack");
                return;
            }
            if (attack.mpCost > player.GetStat(MP))
            {
                await TelegramCommunicator.SendText(chatId, "Not enough MP for that attack");
                return;
            }
            Battler target = player;
            List<Battler> otherAliveBattlers = battlers.Where(x => x != player && x.GetStat(HP) > 0).ToList();
            if (!attack.affectsSelf)
            {
                if (otherAliveBattlers.Count == 1)
                {
                    target = otherAliveBattlers.First();
                }
                else
                {
                    string message = "";
                    if (targetName == null)
                    {
                        message = "Choose a target";
                    }
                    else target = battlers.FirstOrDefault(x => x.name.ToLower() == targetName);
                    if (target == null)
                    {
                        message = "Invalid target";
                    }
                    //if there was a target error, send buttons
                    if (message != "")
                    {
                        List<string> targetNames = new List<string>();
                        foreach (Battler b in otherAliveBattlers) targetNames.Add($"{attack.name} {b.name}");
                        await TelegramCommunicator.SendButtons(chatId, message, targetNames);
                        return;
                    }
                }
            }
            player.upNext = false;
            await UseAttack(chatId, attack, player, target);
        }

        private static async Task UseAttack(string chatId, Attack attack, Battler user, Battler target)
        {
            user.AddToStat(MP, -attack.mpCost);
            user.turnOver = true;
            attack.SetUser(user);
            attack.SetTarget(target);
            float damage = (float)Math.Round(attack.GetDamage(), 2);

            string message = $"{user.name} used {attack.name}!\n";
            if (damage != 0)
            {
                message += $"{target.name} took {damage} damage.\n";
                target.AddToStat(HP, -damage);
            }
            message += attack.OnAttack();

            if (target.GetStat(HP) <= 0)
                await target.OnBehaviour(chatId, target.onKill);

            //check again since onKill events could have healed the target
            if (target.GetStat(HP) <= 0)
            {
                target.turnOver = true;
                await TelegramCommunicator.SendText(chatId, message, true);
                bool isPlayer = (target as Player != null);
                if (!isPlayer)
                {
                    string msg = $"{target.name} died!\n";
                    if (target.droppedItem != null)
                    {
                        msg += $"You obtained {target.droppedItem} x{target.droppedItemAmount}";
                        if (InventorySystem.StringToItem(target.droppedItem, out ObtainableItem droppedItem))
                            await InventorySystem.AddItem(chatId, droppedItem, target.droppedItemAmount);
                    }
                    await TelegramCommunicator.SendText(chatId, msg, true);
                    if (target.experienceGiven != 0)
                    {
                        List<Task> tasks = new List<Task>();
                        foreach (Player p in players) 
                            tasks.Add(p.GainExperience(p.id, target.experienceGiven));
                        await Task.WhenAll(tasks);
                    }
                }
                else await TelegramCommunicator.SendText(chatId, $"{target.name} died!", true);
                List<Battler> side = (isPlayer) ? players.Select(p => (Battler)p).ToList() : enemies;
                //if entire side has been defeated, end battle
                if (side.FirstOrDefault(x => x.GetStat(HP) > 0) == null)
                {
                    await SavePlayerBattle(chatId);
                    battleActive = false;
                    await TelegramCommunicator.RemoveReplyMarkup(chatId, "Battle ends!");
                    //if in party and party wiped out
                    if (partyCode != null && isPlayer)
                    {
                        await PartySystem.WipeOutParty(partyCode, true);
                        await TelegramCommunicator.SendText(chatId, "The whole party died! boo hoo", true);
                    }
                    foreach (Player p in players)
                    {                    
                        await p.OnBattleOver(p.id);
                    }
                    if(partyCode != null)
                        //restore party
                        await PartySystem.WipeOutParty(partyCode, false);
                    await SavePlayerBattle(chatId);
                }
            }
            else
            {
                if (damage != 0) message += $"{target.name} HP: \n{target.GetStatBar(HP)}";
                await TelegramCommunicator.SendText(chatId, message, true);
                await target.OnBehaviour(chatId, target.onHit);
            }

            if (user == GetPlayer(chatId) && battleActive && !battlePaused) 
                await TelegramCommunicator.RemoveReplyMarkup(chatId, "Turn over");

            await SavePlayerBattle(chatId);
            if (battleActive) await NextAttack(chatId);
        }

        //for instances where the battle needs to be paused (such as attack learning)
        public static async Task PauseBattle(string chatId)
        {
            battlePaused = players.FirstOrDefault(x => x.learningAttack != null) != null;
            if(partyCode == null)
                await DatabaseManager.ModifyFieldOfDocument(DbConstants.BATTLE_PAUSED, battlePaused, chatId, DbConstants.COLLEC_PLAYERS);
            else
                await DatabaseManager.ModifyFieldOfDocument(DbConstants.BATTLE_PAUSED, battlePaused, partyCode, DbConstants.COLLEC_PARTIES);
        }

        //call only if battle has been paused
        public static async Task ResumeBattle(string chatId)
        {
            battlePaused = players.FirstOrDefault(x => x.learningAttack != null) != null;
            Dictionary<string, object> playerInfo;
            if (partyCode == null)
            {
                await DatabaseManager.ModifyFieldOfDocument(DbConstants.BATTLE_PAUSED, battlePaused, chatId, DbConstants.COLLEC_PLAYERS);
                playerInfo = await DatabaseManager.GetDocument(chatId, DbConstants.COLLEC_PLAYERS);
            }
            else
            {
                await DatabaseManager.ModifyFieldOfDocument(DbConstants.BATTLE_PAUSED, battlePaused, partyCode, DbConstants.COLLEC_PARTIES);
                playerInfo = await DatabaseManager.GetDocument(partyCode, DbConstants.COLLEC_PARTIES);
            }
            if ((bool)playerInfo[DbConstants.BATTLE_ACTIVE]) await NextAttack(chatId);
        }       

        public static async Task ShowStatus(string chatId, string name = null)
        {
            Battler b = null;

            await LoadPlayerBattle(chatId);

            if (name == null) b = GetPlayer(chatId);
            else if (battleActive) b = battlers.FirstOrDefault(x => x.name == name);

            string friendId = await PartySystem.GetFriendId(chatId, name);
            if (friendId != null)
                b = GetPlayer(friendId);

            if (b == null)
            {
                await TelegramCommunicator.SendText(chatId, "Specified battler doesn't exist.");
                return;
            }

            string s = b.GetStatus();

            await TelegramCommunicator.SendText(chatId, s);
        }
    }
}
