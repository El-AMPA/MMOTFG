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
        private static Dictionary<string, string> enemyDict;
        private static Dictionary<string, Attack> attackDict;
        private static Dictionary<string, ObtainableItem> itemDict;
        private static string defaultPlayer;
        private static JsonSerializerSettings jsonSerializerSettings;

        public static void Init(string enemyPath, string playerPath, string attackPath, string itemPath)
        {
            jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
                DefaultValueHandling = DefaultValueHandling.Populate
            };

            //ignore case to simplify commands
            var comparer = StringComparer.OrdinalIgnoreCase;

            enemyDict = new Dictionary<string, string>(comparer);
            attackDict = new Dictionary<string, Attack>(comparer);
            itemDict = new Dictionary<string, ObtainableItem>(comparer);    

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
                List<Enemy> enemyList = JsonConvert.DeserializeObject<List<Enemy>>(enemyText, 
                    new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate}); //Deserializes the .json file into an array of enemies.
                foreach (Enemy e in enemyList)
                {
                    //serialize and store by name
                    enemyDict.Add(e.name, JsonConvert.SerializeObject(e, jsonSerializerSettings));
                }
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
                List<Attack> attacks = JsonConvert.DeserializeObject<List<Attack>>(attackText,
                    new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate }); //Deserializes the .json file into a list of attacks
                foreach(Attack a in attacks)
                {
                    attackDict.Add(a.name, a);
                }
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
                List<ObtainableItem> items = JsonConvert.DeserializeObject<List<ObtainableItem>>(itemText,
                    new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate }); //Deserializes the .json file into a list of items
                foreach (ObtainableItem i in items)
                {
                    i.OnCreate();
                    itemDict.Add(i.name, i);
                }
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
                //deserialize to check validity
                Player p = JsonConvert.DeserializeObject<Player>(playerText, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate });
                defaultPlayer = JsonConvert.SerializeObject(p, jsonSerializerSettings);
            }
            catch (JsonException e)
            {
                Console.WriteLine("ERROR: player.json isn't formatted correctly. \nError message:" + e.Message);
                Environment.Exit(-1);
            }
        }

        public static Enemy GetEnemy(string name)
        {
            //enemies with names such as "Enemy_1" get simplified to "Enemy"
            name = name.Split('_')[0];
            if (enemyDict.TryGetValue(name, out string s))
            {
                //return by copy to preserve the original             
                Enemy e = JsonConvert.DeserializeObject<Enemy>(s, jsonSerializerSettings);
                e.OnCreate();
                return e;
            }
            else return null;
        }
        public static Player GetDefaultPlayer()
        {
            //return by copy to preserve the original
            Player p = JsonConvert.DeserializeObject<Player>(defaultPlayer, jsonSerializerSettings);
            p.OnCreate();
            return p;
        }

        public static Attack GetAttack(string name)
        {
            if (attackDict.TryGetValue(name, out Attack a))
            {        
                return a;
            }
            else return null;
        }

        public static ObtainableItem GetItem(string name)
        {
            if (itemDict.TryGetValue(name, out ObtainableItem i))
            {
                return i;
            }
            else return null;
        }

        public static List<string> GetAllAttackNames()
        {
            List<string> an = new List<string>();
            foreach (Attack a in attackDict.Values) an.Add(a.name);
            an.Add("skip");
            return an;
        }
    }
}
