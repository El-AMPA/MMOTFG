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
        public static Enemy enemy;
        private static Attack nextPlayerAttack;

        public static List<Battler> battlers;
        public static List<Battler> playerSide;
        public static List<Battler> enemySide;

        public static bool battleActive = false;

        public static void Init()
        {
            player = JSONSystem.GetPlayer();
        }

        public static async Task SavePlayerBattle(long chatId)
        {
            Dictionary<string, object> update = new Dictionary<string, object>();

            update.Add(DbConstants.PLAYER_FIELD_BATTLE_ACTIVE, battleActive);
            update.Add(DbConstants.PLAYER_FIELD_BATTLE_INFO, player.GetSerializable());
            if (!battleActive) { 
                update.Add(DbConstants.PLAYER_FIELD_ENEMY, null); 
            }
            else update.Add(DbConstants.PLAYER_FIELD_ENEMY, (enemySide.First() as Enemy).getSerializable());

            await DatabaseManager.ModifyDocumentFromCollection(update, chatId.ToString(), DbConstants.COLLEC_DEBUG);
        }

        public static async Task LoadPlayerBattle(long chatId)
        {
            Dictionary<string, object> dbPlayer = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID,
                chatId.ToString(), DbConstants.COLLEC_DEBUG);

            battleActive = (bool)dbPlayer[DbConstants.PLAYER_FIELD_BATTLE_ACTIVE];

            player.LoadSerializable((Dictionary<string, object>) dbPlayer[DbConstants.PLAYER_FIELD_BATTLE_INFO]);
            player.SetName((string)dbPlayer[DbConstants.PLAYER_FIELD_NAME]);

            if (dbPlayer[DbConstants.PLAYER_FIELD_ENEMY] != null) {
                string enemyName = ((Dictionary<string, object>)dbPlayer[DbConstants.PLAYER_FIELD_ENEMY])[DbConstants.ENEMY_FIELD_NAME].ToString();
                enemy = JSONSystem.getEnemy(enemyName);
                enemy.loadSerializable((Dictionary<string, object>)dbPlayer[DbConstants.PLAYER_FIELD_ENEMY]);
            }
        }

        public static async Task CreatePlayerBattle(long chatId)
        {
            Dictionary<string, object> update = new Dictionary<string, object>();

            update.Add(DbConstants.PLAYER_FIELD_BATTLE_ACTIVE, false);
            update.Add(DbConstants.PLAYER_FIELD_BATTLE_INFO, player.GetSerializable());
            update.Add(DbConstants.PLAYER_FIELD_ENEMY, null);

            await DatabaseManager.ModifyDocumentFromCollection(update, chatId.ToString(), DbConstants.COLLEC_DEBUG);
        }

        public static async Task<bool> IsPlayerInBattle(long chatId)
        {
            // buscamos en la base de datos para ver el jugador que queremos buscar
            //beep boop beep beep....
            await LoadPlayerBattle(chatId);
            
            return battleActive; 
        }

        public static async Task StartBattle(long chatId, Battler eSide)
        {
            enemySide = new List<Battler>();
            enemySide.Add(eSide);
            battleActive = true;
            if(eSide.imageName != null)
                await TelegramCommunicator.SendImage(chatId, eSide.imageName, eSide.imageCaption);
            await SavePlayerBattle(chatId);
        }

        public static async Task StartBattle(long chatId, List<Enemy> eSide)
        {
            enemySide = new List<Battler>();
            playerSide = new List<Battler>() { player, JSONSystem.getEnemy("Tinky.exe")};
            battlers = new List<Battler>();
            battlers.AddRange(enemySide);
            battlers.AddRange(playerSide);
            battleActive = true;
            List<string> imageNames = new List<string>();
            string caption = "";
            foreach (Battler b in eSide)
            {
                enemySide.Add(b);
                battlers.Add(b);
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
            //await SavePlayerBattle(chatId);
            await NextAttack(chatId);
        }

        public static async Task NextAttack(long chatId)
        {
            //if every battler has finished their turn, start a new turn
            if(battlers.FirstOrDefault(x => x.turnOver == false) == null)
            {
                foreach (Battler ba in battlers)
                {
                    await ba.OnBehaviour(chatId, ba.onTurnEnd);
                    ba.turnOver = false;
                }
            } 
            //sort battlers by speed
            battlers.Sort((b1, b2) => b2.GetStat(SPE).CompareTo(b1.GetStat(SPE)));
            //get first battler that hasn't moved that turn and is alive
            Battler b = battlers.First(x => x.turnOver == false);

            //if the battler is an enemy
            Enemy e = b as Enemy;
            if(e != null)
            {
                Attack a = e.nextAttack();
                Battler target = e;
                if (!a.affectsSelf)
                {
                    //which side is the battler in
                    List<Battler> otherSide = (playerSide.Contains(b)) ? enemySide : playerSide;
                    List<Battler> aliveOtherSide = otherSide.Where(x => x.GetStat(HP) > 0).ToList();
                    target = aliveOtherSide[RNG.Next(0, aliveOtherSide.Count)];
                }
                await UseAttack(chatId, a, e, target);
            }
            //if the battler is a player
            else
            {
                Player p = b as Player;
                p.upNext = true;
                await SetPlayerOptions(chatId);
            }
        }

        public static async Task SetPlayerOptions(long chatId)
        {
            await TelegramCommunicator.SendButtons(chatId, "Your turn", player.attackNum, player.attackNames.ToArray());
        }

        public static async Task PlayerAttack(long chatId, string attackName, string targetName = null)
        {
            //await LoadPlayerBattle(chatId);
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
            if (atkNum == -1) return;
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
                    if (targetName == null || target == null)
                    {
                        nextPlayerAttack = attack;
                        List<string> targetNames = new List<string>();
                        foreach (Battler b in otherAliveBattlers) targetNames.Add($"{attack.name}_{b.name}");
                        await TelegramCommunicator.SendButtons(chatId, message, targetNames.Count, targetNames.ToArray());
                        return;
                    }
                }            
            }         
            player.upNext = false;
            await UseAttack(chatId, attack, player, target);
        }

        private static async Task UseAttack(long chatId, Attack attack, Battler user, Battler target)
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
                battlers.Remove(target);
                await TelegramCommunicator.SendText(chatId, message);
                await target.OnBehaviour(chatId, target.onKill);
                enemy = target as Enemy;
                if(enemy != null)
                {
                    string msg = $"{target.name} died!";
                    if (enemy.droppedMoney > 0)
                        msg += $"\nYou gained {enemy.droppedMoney}€";
                    if (enemy.droppedItem != null)
                    {
                        msg += $"\nYou obtained {enemy.droppedItem} x{enemy.droppedItemAmount}";
                        await InventorySystem.AddItem(chatId, enemy.droppedItem, enemy.droppedItemAmount);
                    }
                    if (enemy.experienceGiven != 0)
                    {
                        await player.GainExperience(chatId, enemy.experienceGiven);
                    }
                    await TelegramCommunicator.SendText(chatId, msg);
                }
                List<Battler> side = (enemySide.Contains(target)) ? enemySide : playerSide;
                //if entire side has been defeated, end battle
                if (side.FirstOrDefault(x => x.GetStat(HP) > 0) == null)
                {
                    battleActive = false;
                    await TelegramCommunicator.RemoveReplyMarkup(chatId);
                    player.OnBattleOver();
                }
            }
            else
            {
                await target.OnBehaviour(chatId, target.onHit);
                if(damage != 0) message += $"\n{target.name} HP: {GetStatBar(target, HP)}";
                await TelegramCommunicator.SendText(chatId, message);
            }

            if (battleActive) await NextAttack(chatId);
            //await SavePlayerBattle(chatId);
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

        public static async Task changePlayerStats(long chatId, StatName stat, float amount)
        {
            player.AddToStat(stat, amount);
            await SavePlayerBattle(chatId);
        }

        public static async Task ShowStatus(long chatId, Battler b)
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
