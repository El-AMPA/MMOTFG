using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static MMOTFG_Bot.StatName;

namespace MMOTFG_Bot
{
    static class BattleSystem
    {
        public static Player player;

        public static List<Battler> battlers;
        public static List<Battler> playerSide;
        public static List<Battler> enemySide;

        public static bool battleActive = false;
        public static bool battlePaused = false;

        private static string partyCode = null;

        public static void Init()
        {
            player = JSONSystem.GetDefaultPlayer();
        }

        public static async Task SavePlayerBattle(string chatId)
        {
            //Save player info in the player field of the databse
            await DatabaseManager.ModifyFieldOfDocument(DbConstants.PLAYER_FIELD_BATTLE_INFO, player.GetSerializable(), chatId, DbConstants.COLLEC_PLAYERS);

            //Data of the battle (battlers, battle state, etc)
            Dictionary<string, object> update = new Dictionary<string, object>();
            update.Add(DbConstants.PLAYER_FIELD_BATTLE_ACTIVE, battleActive);
            update.Add(DbConstants.PLAYER_FIELD_BATTLE_PAUSED, battlePaused);
            if (!battleActive)
            {
                update.Add(DbConstants.PLAYER_FIELD_BATTLER_LIST, null);
            }
            else
            {
                List<Dictionary<string, object>> battlerList = new List<Dictionary<string, object>>();
                foreach (Battler b in battlers)
                {
                    b.isPlayer = await PartySystem.IsInParty(b.name, partyCode);
                    battlerList.Add(b.GetSerializable());
                }
                update.Add(DbConstants.PLAYER_FIELD_BATTLER_LIST, battlerList);
            }

            //If the player is in a party the data is stored in the party database field (or in the player field if this is not the case)
            if(await PartySystem.IsInParty(chatId))
                await DatabaseManager.ModifyDocumentFromCollection(update, await PartySystem.GetPartyCode(chatId), DbConstants.COLLEC_PARTIES);
            else await DatabaseManager.ModifyDocumentFromCollection(update, chatId, DbConstants.COLLEC_PLAYERS);
        }

        public static async Task LoadPlayerBattle(string chatId)
        {
            Dictionary<string, object> dbInfo = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID,
                chatId, DbConstants.COLLEC_PLAYERS);

            player.LoadSerializable((Dictionary<string, object>)dbInfo[DbConstants.PLAYER_FIELD_BATTLE_INFO]);

            if (await PartySystem.IsInParty(chatId))
            {
                partyCode = await PartySystem.GetPartyCode(chatId);
                dbInfo = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PARTY_FIELD_CODE, partyCode, DbConstants.COLLEC_PARTIES);
            }
            else partyCode = null;
                
            battleActive = (bool)dbInfo[DbConstants.PLAYER_FIELD_BATTLE_ACTIVE];
         
            if (!battleActive) return;

            battlePaused = (bool)dbInfo[DbConstants.PLAYER_FIELD_BATTLE_PAUSED];
          
            List<object> dbBattlers = (List<object>)dbInfo[DbConstants.PLAYER_FIELD_BATTLER_LIST];

            battlers = new List<Battler>() { player };
            enemySide = new List<Battler>();
            playerSide = new List<Battler>() { player };

            foreach (Dictionary<string, object> o in dbBattlers)
            {
                Battler b = new Battler();
                if ((bool)o[DbConstants.BATTLER_FIELD_IS_PLAYER])
                {
                    //player has already been loaded
                    if ((string)o[DbConstants.BATTLER_FIELD_NAME] == player.name)
                    {
                        player.LoadSerializableBase(o);
                        continue;
                    }
                }
                else
                {
                    string enemyName = (string)o[DbConstants.BATTLER_FIELD_NAME];
                    b = JSONSystem.GetEnemy(enemyName);
                }
                b.LoadSerializable(o);
                battlers.Add(b);
                if (b.isAlly) playerSide.Add(b);
                else enemySide.Add(b);
            }
        }

        public static async Task CreatePlayerBattleData(string chatId)
        {
            Dictionary<string, object> update = new Dictionary<string, object>();

            //Creamos un jugador nuevo con la info del json
            Player player = JSONSystem.GetDefaultPlayer();
            player.SetName((string)(await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID, chatId, DbConstants.COLLEC_PLAYERS))[DbConstants.PLAYER_FIELD_NAME]);
            update.Add(DbConstants.PLAYER_FIELD_BATTLE_ACTIVE, false);
            update.Add(DbConstants.PLAYER_FIELD_BATTLE_INFO, player.GetSerializable());
            update.Add(DbConstants.PLAYER_FIELD_BATTLER_LIST, null);

            await DatabaseManager.ModifyDocumentFromCollection(update, chatId, DbConstants.COLLEC_PLAYERS);
        }

        public static async Task CreatePartyBattleData(string partyCode)
        {
            Dictionary<string, object> update = new Dictionary<string, object>();

            update.Add(DbConstants.PLAYER_FIELD_BATTLE_ACTIVE, false);
            update.Add(DbConstants.PLAYER_FIELD_BATTLER_LIST, null);

            await DatabaseManager.ModifyDocumentFromCollection(update, partyCode, DbConstants.COLLEC_PARTIES);
        }

        public static async Task<bool> IsPlayerInBattle(string chatId)
        {
            return (bool)await DatabaseManager.GetFieldFromDocument(DbConstants.PLAYER_FIELD_BATTLE_ACTIVE, chatId, DbConstants.COLLEC_PLAYERS);
        }

        public static async Task StartBattle(string chatId, Battler eSide)
        {
            await StartBattle(new List<string>() { chatId }, new List<Battler> { eSide });
        }

        public static async Task StartBattle(List<string> chatIds, List<Battler> eSide)
        {
            await LoadPlayerBattle(chatIds.First());
            enemySide = eSide;
            playerSide = new List<Battler>();
            foreach (string id in chatIds) playerSide.Add(await GetPlayer(id));
            battlers = new List<Battler>();
            battlers.AddRange(enemySide);
            battlers.AddRange(playerSide);
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

                    if (b == eSide.First())
                        caption += b.name;
                    else if (b == eSide.Last())
                        caption += $" and {b.name} appear!";
                    else caption += $", {b.name}";
                }

                await TelegramCommunicator.SendImageCollection(chatIds.First(), imageNames.ToArray(), true);
                await TelegramCommunicator.SendText(chatIds.First(), caption, true);
            }
            //all battlers start being able to move
            foreach (Battler ba in battlers)
            {
                ba.turnOver = false;
                ba.isAlly = playerSide.Contains(ba);
                ba.isPlayer = (ba as Player != null);
            }
            await SavePlayerBattle(chatIds.First());
            await NextAttack(chatIds.First());
        }

        public static async Task NextAttack(string chatId)
        {
            if (!battleActive || battlePaused) return;

            await LoadPlayerBattle(chatId);
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
            if (!b.isPlayer)
            {
                Attack a = b.NextAttack();
                Battler target = b;
                if (!a.affectsSelf)
                {
                    //which side is the battler in
                    List<Battler> otherSide = (b.isAlly) ? enemySide : playerSide;
                    List<Battler> aliveOtherSide = otherSide.Where(x => x.GetStat(HP) > 0).ToList();
                    target = aliveOtherSide[RNG.Next(0, aliveOtherSide.Count)];
                }
                await SavePlayerBattle(chatId);
                await UseAttack(chatId, a, b, target);
            }
            //if the battler is a player
            else
            {
                string friendId = await PartySystem.GetFriendId(chatId, b.name);
                await LoadPlayerBattle(friendId);
                await SetPlayerOptions(friendId, "Select an attack");
            }
        }

        public static async Task SetPlayerOptions(string chatId, string text)
        {
            player = await GetPlayer(chatId);
            player.upNext = true;
            await SavePlayerBattle(chatId);
            await TelegramCommunicator.SendButtons(chatId, text, player.attacks);
        }

        public static async Task PlayerAttack(string chatId, string attackName, string targetName = null)
        {
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
                        //Program.SetAttackKeywords(targetNames);
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

            string message = $"{user.name} used {attack.name}!";
            if (damage != 0)
            {
                message += $" {target.name} took {damage} damage.";
                target.AddToStat(HP, -damage);
            }
            await attack.OnAttack(chatId);

            if (target.GetStat(HP) <= 0)
            {
                target.turnOver = true;
                await TelegramCommunicator.SendText(chatId, message, true);
                await target.OnBehaviour(chatId, target.onKill);
                if (!target.isAlly)
                {
                    string msg = $"{target.name} died!";
                    if (target.droppedItem != null)
                    {
                        msg += $"\nYou obtained {target.droppedItem} x{target.droppedItemAmount}";
                        if (InventorySystem.StringToItem(target.droppedItem, out ObtainableItem droppedItem))
                            await InventorySystem.AddItem(chatId, droppedItem, target.droppedItemAmount);
                    }
                    await TelegramCommunicator.SendText(chatId, msg, true);
                    if (target.experienceGiven != 0)
                    {
                        if (partyCode != null)
                            foreach (string id in await PartySystem.GetPartyMembers(partyCode, true))
                            {
                                player = await GetPlayer(id);
                                await player.GainExperience(id, target.experienceGiven);
                            }
                        else await player.GainExperience(chatId, target.experienceGiven);
                    }
                }
                else await TelegramCommunicator.SendText(chatId, $"{target.name} died!", true);
                List<Battler> side = (target.isAlly) ? playerSide : enemySide;
                //if entire side has been defeated, end battle
                if (side.FirstOrDefault(x => x.GetStat(HP) > 0) == null)
                {
                    await SavePlayerBattle(chatId);
                    battleActive = false;
                    if (partyCode != null)
                    {
                        await PartySystem.WipeOutParty(partyCode, true);
                        await TelegramCommunicator.SendText(chatId, "The whole party died! boo hoo", true);
                        foreach (string id in await PartySystem.GetPartyMembers(partyCode, true))
                        {
                            if (!battlePaused) await TelegramCommunicator.RemoveReplyMarkup(id, "Battle ends!");
                            player = await GetPlayer(id);
                            await player.OnBattleOver(id);
                            await SavePlayerBattle(id);
                        }
                        //restore party
                        await PartySystem.WipeOutParty(partyCode, false);
                    }                        
                    else
                    {
                        if (!battlePaused) await TelegramCommunicator.RemoveReplyMarkup(chatId, "Battle ends!");
                        await player.OnBattleOver(chatId);
                        await SavePlayerBattle(chatId);
                    }
                }
            }
            else
            {
                await target.OnBehaviour(chatId, target.onHit);
                if (damage != 0) message += $"\n{target.name} HP: \n{GetStatBar(target, HP)}";
                await TelegramCommunicator.SendText(chatId, message, true);
            }

            if(user == player) await TelegramCommunicator.RemoveReplyMarkup(chatId, "Turn over");

            await SavePlayerBattle(chatId);
            if (battleActive) await NextAttack(chatId);
        }

        //for instances where the battle needs to be paused (such as move learning)
        public static async Task PauseBattle(string chatId)
        {
            await DatabaseManager.ModifyFieldOfDocument(DbConstants.PLAYER_FIELD_BATTLE_PAUSED, true, chatId, DbConstants.COLLEC_PLAYERS);
        }

        //call only if battle has been paused
        public static async Task ResumeBattle(string chatId)
        {
            await DatabaseManager.ModifyFieldOfDocument(DbConstants.PLAYER_FIELD_BATTLE_PAUSED, false, chatId, DbConstants.COLLEC_PLAYERS);
            var playerInfo = await DatabaseManager.GetDocument(chatId, DbConstants.COLLEC_PLAYERS);
            if ((bool)playerInfo[DbConstants.PLAYER_FIELD_BATTLE_ACTIVE]) await NextAttack(chatId);
        }

        private static string GetStatBar(Battler b, StatName s)
        {
            int green = (int)(10 * b.GetStat(s) / b.GetOriginalStat(s));
            string bar = "";
            for (int i = 0; i < 10; i++)
            {
                if (i <= green) bar += "\U0001F7E9"; //green
                else bar += "\U0001F7E5"; //red
            }
            return bar;
        }

        public static async Task ChangePlayerStats(string chatId, StatName stat, float amount)
        {
            Player player = await GetPlayer(chatId);
            player.AddToStat(stat, amount);
            await SavePlayerBattle(chatId);
        }

        public static async Task ShowStatus(string chatId, string name = null)
        {
            Battler b = null;

            await LoadPlayerBattle(chatId);

            if (name == null) b = player;
            else if (battleActive) b = battlers.FirstOrDefault(x => x.name == name);

            string friendId = await PartySystem.GetFriendId(chatId, name);
            if (friendId != null)
                b = await GetPlayer(friendId);

            if (b == null)
            {
                await TelegramCommunicator.SendText(chatId, "Specified battler doesn't exist.");
                return;
            }

            string s = $"{b.name} Status:\n";
            for (int i = 0; i < Stats.statNum; i++)
            {
                StatName sn = (StatName)i;
                s += $"{sn}: {b.GetStat(sn)}";
                if (Stats.isBounded(sn))
                    s += $"/{b.GetMaxStat(sn)}";
                s += "\n";
            }

            await TelegramCommunicator.SendText(chatId, s);
        }

        private static async Task<Player> GetPlayer(string chatId)
        {
            Player player = JSONSystem.GetDefaultPlayer();
            var info = await DatabaseManager.GetFieldFromDocument(DbConstants.PLAYER_FIELD_BATTLE_INFO, chatId, DbConstants.COLLEC_PLAYERS);
            player.LoadSerializable((Dictionary<string, object>)info);

            if(partyCode != null)
            {
                var dbBattlers = (List<object>)await DatabaseManager.GetFieldFromDocument(DbConstants.PLAYER_FIELD_BATTLER_LIST, partyCode, DbConstants.COLLEC_PARTIES);

                if(dbBattlers != null)
                    foreach (Dictionary<string, object> o in dbBattlers)
                    {
                        //player has already been loaded
                        if ((string)o[DbConstants.BATTLER_FIELD_NAME] == player.name)
                        {
                            player.LoadSerializableBase(o);
                            break;
                        }
                    }
            }
            
            return player;
        }
    }
}
