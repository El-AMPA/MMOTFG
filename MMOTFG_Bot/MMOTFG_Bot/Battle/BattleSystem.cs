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

        public static void Init()
        {
            rnd = new Random();
            player = new Player();
        }

        public static async void startBattle(long chatId, Enemy e)
        {
            enemy = e;
            await TelegramCommunicator.SendImage(chatId, e.imageName, e.imageCaption);
            Console.WriteLine("tiroriroriroriro chan chan chan");
        }

        public static async void setPlayerOptions(long chatId)
        {
            await TelegramCommunicator.SendButtons(chatId, player.attackNum, player.attackNames.ToArray());
        }

        public static async void playerAttack(long chatId, string attackName)
        {
            attackName = char.ToUpper(attackName[0]) + attackName.Substring(1);
            int attack = player.attackNames.IndexOf(attackName);
            if (attack == -1) return;
            float damage = player.stats[(int)StatNames.ATK] * player.attacks[attack].power;
            await TelegramCommunicator.SendText(chatId, "Player used " + player.attacks[attack].name + "! Enemy took " + damage + " damage.");
            enemy.stats[(int)StatNames.HP] -= damage;
            await TelegramCommunicator.SendText(chatId, "Enemy has " + enemy.stats[(int)StatNames.HP] + " HP left");
            if (enemy.stats[(int)StatNames.HP] <= 0)
                await TelegramCommunicator.SendText(chatId, "Enemy died!");
            else enemyAttack(chatId);
        }

        private static async void enemyAttack(long chatId)
        {
            int attack = rnd.Next(0, enemy.attackNum);
            float damage = enemy.stats[(int)StatNames.ATK] * enemy.attacks[attack].power;
            await TelegramCommunicator.SendText(chatId, "Enemy used " + enemy.attacks[attack].name + "! Player took " + damage + " damage.");
            player.stats[(int)StatNames.HP] -= damage;
            await TelegramCommunicator.SendText(chatId, "You have " + player.stats[(int)StatNames.HP] + " HP left");
            if (player.stats[(int)StatNames.HP] <= 0)
                await TelegramCommunicator.SendText(chatId, "You died!");
        }
    }
}
