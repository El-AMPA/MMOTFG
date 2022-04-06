using System;
using System.Collections.Generic;
using System.Text;
using static MMOTFG_Bot.StatName;
using System.Threading.Tasks;
using System.Linq;

namespace MMOTFG_Bot
{
    static class BattleSystem
    {
        public static Player player;
        //public static Enemy enemy;

        public static List<Battler> battlers;
        public static List<Battler> playerSide;
        public static List<Battler> enemySide;

        public static bool battleActive = false;
        public static bool battlePaused = false;

        public static void Init()
        {
            player = JSONSystem.GetPlayer();
        }

        public static async Task SavePlayerBattle(string chatId)
        {
            Dictionary<string, object> update = new Dictionary<string, object>();

            update.Add(DbConstants.PLAYER_FIELD_BATTLE_ACTIVE, battleActive);
            update.Add(DbConstants.PLAYER_FIELD_BATTLE_INFO, player.GetSerializable());
            if (!battleActive)
            {
                update.Add(DbConstants.PLAYER_FIELD_BATTLER_LIST, null);
            }
            else
            {
                List<Dictionary<string, object>> battlerList = new List<Dictionary<string, object>>();
                foreach (Battler b in battlers)
                {     
                    //player has already been saved
                    if(b.name != player.name)
                        battlerList.Add(b.GetSerializable());
                }
                update.Add(DbConstants.PLAYER_FIELD_BATTLER_LIST, battlerList);
            }

            await DatabaseManager.ModifyDocumentFromCollection(update, chatId.ToString(), DbConstants.COLLEC_DEBUG);
        }

        public static async Task LoadPlayerBattle(string chatId)
        {
            Dictionary<string, object> dbPlayer = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID,
                chatId.ToString(), DbConstants.COLLEC_DEBUG);

            battleActive = (bool)dbPlayer[DbConstants.PLAYER_FIELD_BATTLE_ACTIVE];

            player.LoadSerializable((Dictionary<string, object>) dbPlayer[DbConstants.PLAYER_FIELD_BATTLE_INFO]);
            player.SetName((string)dbPlayer[DbConstants.PLAYER_FIELD_NAME]);

            if (!battleActive) return;

            List<object> dbBattlers = (List<object>)dbPlayer[DbConstants.PLAYER_FIELD_BATTLER_LIST];

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
                        continue;
                    }
                }
                else
                {
                    string enemyName = (string)o[DbConstants.BATTLER_FIELD_NAME];
                    b = JSONSystem.getEnemy(enemyName);
                }
                b.LoadSerializable(o);
                battlers.Add(b);
                if (b.isAlly) playerSide.Add(b);
                else enemySide.Add(b);
            }
        }

        public static async Task CreatePlayerBattle(string chatId)
        {
            Dictionary<string, object> update = new Dictionary<string, object>();

            update.Add(DbConstants.PLAYER_FIELD_BATTLE_ACTIVE, false);
            update.Add(DbConstants.PLAYER_FIELD_BATTLE_INFO, player.GetSerializable());
            update.Add(DbConstants.PLAYER_FIELD_BATTLER_LIST, null);

            await DatabaseManager.ModifyDocumentFromCollection(update, chatId.ToString(), DbConstants.COLLEC_DEBUG);
        }

        public static async Task<bool> IsPlayerInBattle(string chatId)
        {
            //await LoadPlayerBattle(chatId);
            //return battleActive;

            return (bool)await DatabaseManager.GetFieldFromDocument(DbConstants.PLAYER_FIELD_BATTLE_ACTIVE, chatId.ToString(), DbConstants.COLLEC_DEBUG);
        }

        public static async Task StartBattle(string chatId, Battler eSide)
        {
            await StartBattle(chatId, new List<Battler> { eSide });
        }

        public static async Task StartBattle(string chatId, List<Battler> eSide)
        {
            enemySide = eSide;
            playerSide = new List<Battler>() { player };
            battlers = new List<Battler>();
            battlers.AddRange(enemySide);
            battlers.AddRange(playerSide);
            battleActive = true;
            battlePaused = false;
            if(eSide.Count == 1)
            {
                Battler b = eSide.First();
                if (b.imageName != null)
                    await TelegramCommunicator.SendImage(chatId, b.imageName, b.imageCaption);
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
                await TelegramCommunicator.SendImageCollection(chatId, imageNames.ToArray());
                await TelegramCommunicator.SendText(chatId, caption);
            }
            //all battlers start being able to move
            foreach (Battler ba in battlers)
            {
                ba.turnOver = false;
                ba.isAlly = playerSide.Contains(ba);
                ba.isPlayer = (ba as Player != null);
            }
            await SavePlayerBattle(chatId);
            await NextAttack(chatId);
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

            //if the battler is an enemy
            if((b as Player) == null)
            {
                await TelegramCommunicator.SendText(chatId, $"{b.name}'s turn");
                Attack a = b.nextAttack();
                Battler target = b;
                if (!a.affectsSelf)
                {
                    //which side is the battler in
                    List<Battler> otherSide = (b.isAlly) ? enemySide : playerSide;
                    List<Battler> aliveOtherSide = otherSide.Where(x => x.GetStat(HP) > 0).ToList();
                    target = aliveOtherSide[RNG.Next(0, aliveOtherSide.Count)];
                }
                await UseAttack(chatId, a, b, target);
            }
            //if the battler is a player
            else
            {
                Player p = b as Player;
                p.upNext = true;
                await SetPlayerOptions(chatId, "Your turn");
            }
        }

        public static async Task SetPlayerOptions(string chatId, string text)
        {
            await TelegramCommunicator.SendButtons(chatId, text, player.attackNames.ToArray());
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
            attackName = char.ToUpper(attackName[0]) + attackName.Substring(1);
            int atkNum = player.attackNames.IndexOf(attackName);
            if (atkNum == -1)
            {
                await TelegramCommunicator.SendText(chatId, "Invalid attack");
                return;
            }
            Attack attack = player.attacks[atkNum];
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
                        await TelegramCommunicator.SendButtons(chatId, message, targetNames.ToArray());
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
                await TelegramCommunicator.SendText(chatId, message);
                await target.OnBehaviour(chatId, target.onKill);
                if(!target.isAlly)
                {
                    string msg = $"{target.name} died!";
                    if (target.droppedMoney > 0)
                        msg += $"\nYou gained {target.droppedMoney}€";
                    if (target.droppedItem != null)
                    {
                        msg += $"\nYou obtained {target.droppedItem} x{target.droppedItemAmount}";
                        await InventorySystem.AddItem(chatId, target.droppedItem, target.droppedItemAmount);
                    }                    
                    await TelegramCommunicator.SendText(chatId, msg);
                    if (target.experienceGiven != 0)
                    {
                        await player.GainExperience(chatId, target.experienceGiven);
                    }
                }
                List<Battler> side = (target.isAlly) ? playerSide : enemySide;
                //if entire side has been defeated, end battle
                if (side.FirstOrDefault(x => x.GetStat(HP) > 0) == null)
                {
                    battleActive = false;
                    if(!battlePaused) await TelegramCommunicator.RemoveReplyMarkup(chatId);
                    await player.OnBattleOver(chatId);
                }
            }
            else
            {
                await target.OnBehaviour(chatId, target.onHit);
                if(damage != 0) message += $"\n{target.name} HP: {GetStatBar(target, HP)}";
                await TelegramCommunicator.SendText(chatId, message);
            }

            await SavePlayerBattle(chatId);
            if (battleActive) await NextAttack(chatId);
        }

        //for instances where the battle needs to be paused (such as move learning)
        public static async Task PauseBattle(string chatId)
        {
            await SavePlayerBattle(chatId);
            battlePaused = true;
        }

        //call only if battle has been paused
        public static async Task ResumeBattle(string chatId)
        {
            battlePaused = false;
            if (battleActive) await NextAttack(chatId);
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

        public static async Task changePlayerStats(string chatId, StatName stat, float amount)
        {
            player.AddToStat(stat, amount);
            await SavePlayerBattle(chatId);
        }

        public static async Task ShowStatus(string chatId, Battler b)
        {
            await LoadPlayerBattle(chatId);
            if (!battleActive && b != player){
                await TelegramCommunicator.SendText(chatId, "No battle currently active");
                return;
            }
            string s = $"{b.name} Status:\n";
            for(int i = 0; i < Stats.statNum; i++)
            {
                StatName sn = (StatName)i;
                s += $"{sn}: {b.GetStat(sn)}";
                if (Stats.isBounded(sn))
                    s += $"/{b.GetMaxStat(sn)}";
                s += "\n";
            }

            await TelegramCommunicator.SendText(chatId, s);
        }
    }
}
