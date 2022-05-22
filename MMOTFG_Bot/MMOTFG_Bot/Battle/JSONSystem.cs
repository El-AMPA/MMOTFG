using Newtonsoft.Json;
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
        private static List<Attack> attacks;
        private static List<ObtainableItem> items;
        private static Player defaultPlayer;

        public static void Init(string enemyPath, string playerPath, string attackPath, string itemPath)
        {
            ReadAttacksFromJSON(attackPath);
            ReadItemsFromJSON(itemPath);
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

        private static void ReadAttacksFromJSON(string path)
        {
            string attackText = ""; //Text of the entire .json file
            try
            {
                attackText = File.ReadAllText(path, Encoding.GetEncoding("iso-8859-1")); //This encoding supports spanish characters "ñ, á ..."
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("ERROR: attacks.json couldn't be found in assets folder.");
                Environment.Exit(-1);
            }

            try
            {
                attacks = JsonConvert.DeserializeObject<List<Attack>>(attackText,
                    new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate }); //Deserializes the .json file into a list of attacks
            }
            catch (JsonException e)
            {
                Console.WriteLine("ERROR: attacks.json isn't formatted correctly. \nError message:" + e.Message);
                Environment.Exit(-1);
            }
        }

        private static void ReadItemsFromJSON(string path)
        {
            string itemText = ""; //Text of the entire .json file
            try
            {
                itemText = File.ReadAllText(path, Encoding.GetEncoding("iso-8859-1")); //This encoding supports spanish characters "ñ, á ..."
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("ERROR: items.json couldn't be found in assets folder.");
                Environment.Exit(-1);
            }

            try
            {
                items = JsonConvert.DeserializeObject<List<ObtainableItem>>(itemText,
                    new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate }); //Deserializes the .json file into a list of items
                foreach (ObtainableItem i in items) i.OnCreate();
            }
            catch (JsonException e)
            {
                Console.WriteLine("ERROR: items.json isn't formatted correctly. \nError message:" + e.Message);
                Environment.Exit(-1);
            }
        }

        public static void ReadPlayerFromJSON(string playerPath)
        {
            string playerText = ""; //Text of the entire .json file
            try
            {
                playerText = File.ReadAllText(playerPath, Encoding.GetEncoding("iso-8859-1")); //This encoding supports spanish characters "ñ, á ..."
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("ERROR: player.json couldn't be found in assets folder.");
                Environment.Exit(-1);
            }

            try
            {
                defaultPlayer = JsonConvert.DeserializeObject<Player>(playerText,
                    new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate }); //Deserializes the .json file into a player
                defaultPlayer.OnCreate();
            }
            catch (JsonException e)
            {
                Console.WriteLine("ERROR: player.json isn't formatted correctly. \nError message:" + e.Message);
                Environment.Exit(-1);
            }
        }

        public static Battler GetEnemy(string name)
        {
            Battler e = enemies.FirstOrDefault(x => x.name.ToLower() == name.ToLower());
            if (e != null)
            {
                //return by copy to preserve the original
                return (Battler)e.Clone();
            }
            else return e;
        }
        public static Player GetDefaultPlayer()
        {
            //return by copy to preserve the original
            return (Player)defaultPlayer.Clone();
        }

        public static Attack GetAttack(string name)
        {
            return attacks.FirstOrDefault(x => x.name.ToLower() == name.ToLower());
        }

        public static ObtainableItem GetItem(string name)
        {
            return items.FirstOrDefault(x => x.name.ToLower() == name.ToLower());
        }

        public static List<string> GetAllAttackNames()
        {
            List<string> an = new List<string>();
            foreach (Attack a in attacks) an.Add(a.name);
            an.Add("skip");
            return an;
        }
    }
}
