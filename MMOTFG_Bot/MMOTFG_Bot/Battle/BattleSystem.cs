using System;
using System.Collections.Generic;
using System.Text;
using static MMOTFG_Bot.StatName;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    static class BattleSystem
    {
        public static Player player;
        public static Enemy enemy;

        public static bool battleActive = false;

        public static void Init()
        {
            player = new Player();
            enemy = new Enemy();
        }

        public static async Task SavePlayerBattle(long chatId)
        {
            Dictionary<string, object> update = new Dictionary<string, object>();

            update.Add(DbConstants.PLAYER_FIELD_BATTLE_ACTIVE, battleActive);
            update.Add(DbConstants.PLAYER_FIELD_BATTLE_INFO, player.getSerializable());
            if (enemy == null) { 
                update.Add(DbConstants.PLAYER_FIELD_ENEMY, null); 
            }
            else update.Add(DbConstants.PLAYER_FIELD_ENEMY, enemy.getSerializable());

            await DatabaseManager.ModifyDocumentFromCollection(update, chatId.ToString(), DbConstants.COLLEC_DEBUG);
        }

        public static async Task LoadPlayerBattle(long chatId)
        {
            Dictionary<string, object> dbPlayer = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID,
                chatId.ToString(), DbConstants.COLLEC_DEBUG);

            battleActive = (bool)dbPlayer[DbConstants.PLAYER_FIELD_BATTLE_ACTIVE];

            player.loadSerializable((Dictionary<string, object>) dbPlayer[DbConstants.PLAYER_FIELD_BATTLE_INFO]);
            player.setName((string)dbPlayer[DbConstants.PLAYER_FIELD_NAME]);

            if (dbPlayer[DbConstants.PLAYER_FIELD_ENEMY] != null) {
                string enemyName = ((Dictionary<string, object>)dbPlayer[DbConstants.PLAYER_FIELD_ENEMY])[DbConstants.ENEMY_FIELD_NAME].ToString();
                enemy = MMOTFG_Bot.Battle.Enemies.EnemySystem.getEnemy(enemyName);
                enemy.loadSerializable((Dictionary<string, object>)dbPlayer[DbConstants.PLAYER_FIELD_ENEMY]);
            }
        }

        public static async Task CreatePlayerBattle(long chatId)
        {
            Dictionary<string, object> update = new Dictionary<string, object>();

            update.Add(DbConstants.PLAYER_FIELD_BATTLE_ACTIVE, false);
            update.Add(DbConstants.PLAYER_FIELD_BATTLE_INFO, player.getSerializable());
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

        public static async void startBattle(long chatId, Enemy e)
        {
            enemy = e;
            battleActive = true;
            if(enemy.imageName != null)
                await TelegramCommunicator.SendImage(chatId, e.imageName, e.imageCaption);
            setPlayerOptions(chatId);
            await SavePlayerBattle(chatId);
        }

        public static async void setPlayerOptions(long chatId)
        {
            //List<string> attacksWithMP = new List<string>();
            //for(int i = 0; i < player.attackNum; i++)
            //{
            //    attacksWithMP.Add(player.attackNames[i] + "\nMP: " + player.attackmpCosts[i]);
            //}
            await TelegramCommunicator.SendButtons(chatId, player.attackNum, player.attackNames.ToArray());
        }

        public static async void playerAttack(long chatId, string attackName)
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
            if (attack.mpCost > player.getStat(MP))
            {
                await TelegramCommunicator.SendText(chatId, "Not enough MP for that attack");
                return;
            }

            useAttack(chatId, attack, player, enemy); 
        }

        private static async void enemyAttack(long chatId)
        {
            Attack attack = enemy.nextAttack();

            useAttack(chatId, attack, enemy, player);
        }

        private static async void useAttack(long chatId, Attack attack, Battler user, Battler target)
        {
            user.changeStat(MP, -attack.mpCost);
            attack.setUser(user);
            attack.setTarget(target);
            float damage = (float)Math.Round(attack.getDamage(), 2);

            string message = $"{user.name} used {attack.name}!";
            if (damage != 0)
            {
                message += $" {target.name} took {damage} damage.";
                target.changeStat(HP, -damage);
            }
            await TelegramCommunicator.SendText(chatId, message);
            attack.OnAttack(chatId);

            if (target.getStat(HP) <= 0)
            {
                target.OnKill(chatId);
                battleActive = false;
                if(target == enemy)
                {
                    string msg = $"{target.name} died!";
                    if (enemy.droppedMoney > 0)
                        msg += $"\nYou gained {enemy.droppedMoney}€";
                    if (enemy.droppedItem != null)
                    {
                        msg += $"\nYou obtained {enemy.droppedItem.name} x{enemy.droppedItemAmount}";
                        await InventorySystem.AddItem(chatId, enemy.droppedItem.name, enemy.droppedItemAmount);
                    }
                    await TelegramCommunicator.SendText(chatId, msg);
                }
                await TelegramCommunicator.RemoveReplyMarkup(chatId);
                player.OnBattleOver();
                enemy = null;
            }
            else
            {
                target.OnHit(chatId);
                if(damage != 0) await TelegramCommunicator.SendText(chatId, $"{target.name} HP: {getStatBar(target, HP)}");
                if(user == player) enemyAttack(chatId);
                else
                {
                    user.OnTurnEnd(chatId);
                    target.OnTurnEnd(chatId);
                }
            }

            await SavePlayerBattle(chatId);
        }

        private static string getStatBar(Battler b, StatName s)
        {
            int green = (int)(10 * b.getStat(s) / b.getOriginalStat(s));
            string bar = "";
            for (int i = 0; i < 10; i++)
            {
                if (i < green) bar += "\U0001F7E9"; //green
                else bar += "\U0001F7E5"; //red
            }
            return bar;
        }

        public async static Task showStatus(long chatId, Battler b)
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
                s += $"{Enum.GetName(typeof(StatName), i)}: {b.getStat(sn)}";
                if (sn == HP || sn == MP)
                    s += $"/{b.getOriginalStat(sn)}";
                s += "\n";
            }

            await TelegramCommunicator.SendText(chatId, s);
        }
    }
}
