﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MMOTFG_Bot
{
    static class JSONSystem
    {
        private static List<Battler> enemies;
        private static Player player;
        private static List<Attack> attacks;

        public static void Init(string enemyPath, string playerPath, string attackPath)
        {
            ReadAttacksFromJSON(attackPath);
            ReadEnemiesFromJSON(enemyPath);
            ReadPlayerFromJSON(playerPath);
        }

        /// <summary>
        /// Deserializes the .json file specified by path and constructs the enemy list.
        /// </summary>
        private static void ReadEnemiesFromJSON(string path)
        {
            string enemyText = ""; //Text of the entire .json file
            try
            {
                enemyText = File.ReadAllText(path, Encoding.GetEncoding("iso-8859-1")); //This encoding supports spanish characters "ñ, á ..."
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("ERROR: enemies.json couldn't be found in assets folder.");
                Environment.Exit(-1);
            }

            try
            {
                enemies = JsonConvert.DeserializeObject<List<Battler>>(enemyText, 
                    new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate}); //Deserializes the .json file into an array of enemies.
                foreach (Battler e in enemies) e.OnCreate();
            }
            catch (JsonException e)
            {
                Console.WriteLine("ERROR: enemies.json isn't formatted correctly. \nError message:" + e.Message);
                Environment.Exit(-1);
            }
        }

        private static void ReadPlayerFromJSON(string path)
        {
            string playerText = ""; //Text of the entire .json file
            try
            {
                playerText = File.ReadAllText(path, Encoding.GetEncoding("iso-8859-1")); //This encoding supports spanish characters "ñ, á ..."
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("ERROR: player.json couldn't be found in assets folder.");
                Environment.Exit(-1);
            }

            try
            {
                player = JsonConvert.DeserializeObject<Player>(playerText,
                    new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate }); //Deserializes the .json file into a player
                player.OnCreate();
                player.AfterCreate();
            }
            catch (JsonException e)
            {
                Console.WriteLine("ERROR: player.json isn't formatted correctly. \nError message:" + e.Message);
                Environment.Exit(-1);
            }
        }

        private static void ReadAttacksFromJSON(string path)
        {
            string playerText = ""; //Text of the entire .json file
            try
            {
                playerText = File.ReadAllText(path, Encoding.GetEncoding("iso-8859-1")); //This encoding supports spanish characters "ñ, á ..."
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("ERROR: attacks.json couldn't be found in assets folder.");
                Environment.Exit(-1);
            }

            try
            {
                attacks = JsonConvert.DeserializeObject<List<Attack>>(playerText,
                    new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate }); //Deserializes the .json file into a list of attacks
            }
            catch (JsonException e)
            {
                Console.WriteLine("ERROR: attacks.json isn't formatted correctly. \nError message:" + e.Message);
                Environment.Exit(-1);
            }
        }

        public static Battler GetEnemy(string name)
        {
            Battler e = enemies.FirstOrDefault(x => x.name == name);
            if (e != null)
            {
                e.New();
                return e;
            }
            else return enemies.First();
        }

        public static Player GetPlayer()
        {
            return player;
        }

        public static Attack GetAttack(string name)
        {
            Attack a = attacks.FirstOrDefault(x => x.name == name);
            if (a != null)
            {
                return a;
            }
            else return attacks.First();
        }
    }
}
