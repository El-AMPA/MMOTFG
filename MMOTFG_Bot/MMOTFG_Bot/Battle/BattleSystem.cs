using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    static class BattleSystem
    {
        public static Player player;
        public static Enemy enemy;
        static Random rnd;

        public static bool battleActive = false;

        public static void Init()
        {
            rnd = new Random();
            player = new Player();
        }

        public static async void startBattle(long chatId, Enemy e)
        {
            enemy = e;
            battleActive = true;
            await TelegramCommunicator.SendImage(chatId, e.imageName, e.imageCaption);
            Console.WriteLine("tiroriroriroriro chan chan chan");
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
            player.stats[(int)StatName.MP] -= attack.mpCost;
            float damage = player.stats[(int)StatName.ATK] * attack.power;
            await TelegramCommunicator.SendText(chatId, $"Player used {attack.name}! Enemy took {damage} damage.");          
            //await TelegramCommunicator.SendText(chatId, $"Remaining MP: {player.stats[(int)StatNames.MP]}");
            enemy.stats[(int)StatName.HP] -= damage;
            enemy.OnHit(chatId);
            await TelegramCommunicator.SendText(chatId, $"Enemy HP: {getStatBar(enemy, StatName.HP)}");
            //await TelegramCommunicator.SendText(chatId, $"Enemy has {enemy.stats[(int)StatNames.HP]} HP left");
            if (enemy.stats[(int)StatName.HP] <= 0)
            {
                battleActive = false;
                string msg = "Enemy died!";
                if (enemy.droppedMoney > 0)
                    msg += $"\nYou gained {enemy.droppedMoney}€";
                if (enemy.droppedItem != null)
                {
                    msg += $"\nYou obtained {enemy.droppedItem.name} x{enemy.droppedItemAmount}";
                    InventorySystem.AddItem(chatId, enemy.droppedItem.name, enemy.droppedItemAmount);
                }
                await TelegramCommunicator.SendText(chatId, msg);
            }
            else enemyAttack(chatId);
        }

        private static async void enemyAttack(long chatId)
        {
            Attack attack = enemy.nextAttack(rnd);
            float damage = enemy.stats[(int)StatName.ATK] * attack.power;
            await TelegramCommunicator.SendText(chatId, $"Enemy used {attack.name}! Player took {damage} damage.");
            player.stats[(int)StatName.HP] -= damage;
            await TelegramCommunicator.SendText(chatId, $"Your HP: {getStatBar(player, StatName.HP)}");
            //await TelegramCommunicator.SendText(chatId, $"You have {player.stats[(int)StatNames.HP]} HP left");
            if (player.stats[(int)StatName.HP] <= 0)
                await TelegramCommunicator.SendText(chatId, "You died!");
            else enemy.OnTurnEnd(chatId);
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
            string s = "";
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
