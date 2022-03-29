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
            SetPlayerOptions(chatId);
            await SavePlayerBattle(chatId);
        }

        public static async Task StartBattle(long chatId, List<Enemy> eSide)
        {
            enemySide = new List<Battler>();
            battleActive = true;
            List<string> imageNames = new List<string>();
            string caption = "";
            foreach (Battler b in eSide)
            {
                enemySide.Add(b);
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
            SetPlayerOptions(chatId);
            await SavePlayerBattle(chatId);
        }

        public static async Task nextAttack(long chatId)
        {
            //if every battler has finished their turn, start a new turn
            if(battlers.First(x => x.turnOver == false) == null)
            {
                foreach (Battler ba in battlers) ba.turnOver = false;
            } 
            //sort battlers by speed
            battlers.Sort((b1, b2) => b2.GetStat(SPE).CompareTo(b1.GetStat(SPE)));
            //get first battler that hasn't moved that turn
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

            }
        }

        public static async void SetPlayerOptions(long chatId)
        {
            await TelegramCommunicator.SendButtons(chatId, player.attackNum, player.attackNames.ToArray());
        }

        public static async Task PlayerAttack(long chatId, string attackName)
        {
            await LoadPlayerBattle(chatId);
            if (!battleActive)
            {
                await TelegramCommunicator.SendText(chatId, "No battle currently active");
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

            await UseAttack(chatId, attack, player, enemy); 
        }

        public static async Task EnemyAttack(long chatId)
        {
            Attack attack = enemy.nextAttack();

            await UseAttack(chatId, attack, enemy, player);
        }

        private static async Task UseAttack(long chatId, Attack attack, Battler user, Battler target)
        {
            user.AddToStat(MP, -attack.mpCost);
            attack.SetUser(user);
            attack.SetTarget(target);
            float damage = (float)Math.Round(attack.GetDamage(), 2);

            string message = $"{user.name} used {attack.name}!";
            if (damage != 0)
            {
                message += $" {target.name} took {damage} damage.";
                target.AddToStat(HP, -damage);
            }
            await TelegramCommunicator.SendText(chatId, message);
            await attack.OnAttack(chatId);

            if (target.GetStat(HP) <= 0)
            {
                await target.OnBehaviour(chatId, target.onKill);
                battleActive = false;
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
                await TelegramCommunicator.RemoveReplyMarkup(chatId);
                player.OnBattleOver();
                enemy = null;
            }
            else
            {
                await target.OnBehaviour(chatId, target.onHit);
                if(damage != 0) await TelegramCommunicator.SendText(chatId, $"{target.name} HP: {GetStatBar(target, HP)}");
                if(user == player) await EnemyAttack(chatId);
                else
                {
                    await user.OnBehaviour(chatId, user.onTurnEnd);
                    await target.OnBehaviour(chatId, target.onTurnEnd);
                }
            }

            await SavePlayerBattle(chatId);
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
