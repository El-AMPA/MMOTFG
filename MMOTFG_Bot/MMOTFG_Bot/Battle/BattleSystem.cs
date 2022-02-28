using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    static class BattleSystem
    {
        static Player player;
        static Enemy enemy;
        static Random rnd;

        static bool battleActive = false;

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
            if (attack.mpCost > player.stats[(int)StatNames.MP])
            {
                await TelegramCommunicator.SendText(chatId, "Not enough MP for that attack");
                return;
            }
            player.stats[(int)StatNames.MP] -= attack.mpCost;
            float damage = player.stats[(int)StatNames.ATK] * attack.power;
            await TelegramCommunicator.SendText(chatId, $"Player used {attack.name}! Enemy took {damage} damage.");
            await TelegramCommunicator.SendText(chatId, $"Remaining MP: {player.stats[(int)StatNames.MP]}");
            enemy.stats[(int)StatNames.HP] -= damage;
            await TelegramCommunicator.SendText(chatId, $"Enemy has {enemy.stats[(int)StatNames.HP]} HP left");
            if (enemy.stats[(int)StatNames.HP] <= 0)
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
            float damage = enemy.stats[(int)StatNames.ATK] * attack.power;
            await TelegramCommunicator.SendText(chatId, $"Enemy used {attack.name}! Player took {damage} damage.");
            player.stats[(int)StatNames.HP] -= damage;
            await TelegramCommunicator.SendText(chatId, $"You have {player.stats[(int)StatNames.HP]} HP left");
            if (player.stats[(int)StatNames.HP] <= 0)
                await TelegramCommunicator.SendText(chatId, "You died!");
        }
    }
}
