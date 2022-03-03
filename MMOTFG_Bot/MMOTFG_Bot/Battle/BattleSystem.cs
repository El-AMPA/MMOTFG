using System;
using System.Collections.Generic;
using System.Text;

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
        }

        public static async void startBattle(long chatId, Enemy e)
        {
            enemy = e;
            battleActive = true;
            await TelegramCommunicator.SendImage(chatId, e.imageName, e.imageCaption);
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
            if (!battleActive)
            {
                await TelegramCommunicator.SendText(chatId, "No battle currently active");
                return;
            }
            attackName = char.ToUpper(attackName[0]) + attackName.Substring(1);
            int atkNum = player.attackNames.IndexOf(attackName);
            if (atkNum == -1) return;
            Attack attack = player.attacks[atkNum];
            if (attack.mpCost > player.stats[(int)StatName.MP])
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
            user.stats[(int)StatName.MP] -= attack.mpCost;
            attack.setUser(user);
            attack.setTarget(target);
            float damage = (float)Math.Round(attack.getDamage(), 2);

            string message = $"{user.name} used {attack.name}!";
            if (damage != 0)
            {
                message += $" {target.name} took {damage} damage.";
                target.stats[(int)StatName.HP] -= damage;
            }
            await TelegramCommunicator.SendText(chatId, message);
            attack.OnAttack(chatId);

            if (target.stats[(int)StatName.HP] <= 0)
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
            }
            else
            {
                target.OnHit(chatId);
                if(damage != 0) await TelegramCommunicator.SendText(chatId, $"{target.name} HP: {getStatBar(target, StatName.HP)}");
                if(user == player) enemyAttack(chatId);
                else
                {
                    user.OnTurnEnd(chatId);
                    target.OnTurnEnd(chatId);
                }
            }
        }

        private static string getStatBar(Battler b, StatName s)
        {
            int green = (int)(10 * b.stats[(int)s] / b.originalStats[(int)s]);
            string bar = "";
            for (int i = 0; i < 10; i++)
            {
                if (i < green) bar += "\U0001F7E9"; //green
                else bar += "\U0001F7E5"; //red
            }
            return bar;
        }

        public static async void showStatus(long chatId, Battler b)
        {
            if (!battleActive && b != player){
                await TelegramCommunicator.SendText(chatId, "No battle currently active");
                return;
            }
            string s = $"{b.name} Status:\n";
            for(int i = 0; i < Stats.statNum; i++)
            {
                StatName sn = (StatName)i;
                s += $"{Enum.GetName(typeof(StatName), i)}: {b.stats[i]}";
                if (sn == StatName.HP || sn == StatName.MP)
                    s += $"/{b.originalStats[i]}";
                s += "\n";
            }

            await TelegramCommunicator.SendText(chatId, s);
        }
    }
}
