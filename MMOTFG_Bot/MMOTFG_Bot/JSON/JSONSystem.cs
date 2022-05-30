using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MMOTFG_Bot.Battle;
using MMOTFG_Bot.Inventory;
using MMOTFG_Bot.Navigation;

namespace MMOTFG_Bot.Loader
{
    static class JSONSystem
    {
        private static Dictionary<string, string> enemyDict;
        private static Dictionary<string, Attack> attackDict;
        private static Dictionary<string, ObtainableItem> itemDict;
        private static string defaultPlayer;
        private static List<Node> nodes;
        private static Dictionary<string, string> directionSynonyms;
        private static JsonSerializerSettings jsonSerializerSettings;

        public static void Init(string mapPath, string enemyPath, string playerPath, string attackPath, string itemPath, string synonymsPath)
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
            directionSynonyms = new Dictionary<string, string>(comparer);

            ReadAttacksFromJSON(attackPath);
            ReadItemsFromJSON(itemPath);
            ReadEnemiesFromJSON(enemyPath);
            ReadPlayerFromJSON(playerPath);
            ReadMapFromJSON(mapPath);
            ReadDirectionsSynonymsFromJSON(synonymsPath);
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

        /// <summary>
        /// Deserializes the .json file specified by path and constructs the map.
        /// </summary>
        private static void ReadMapFromJSON(string mapPath)
        {
            //Map reading
            string mapText = ""; //Text of the entire .json file of the map
            try
            {
                mapText = File.ReadAllText(mapPath, Encoding.GetEncoding(65001)); // Encoding: UTF-8
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("ERROR: map.json couldn't be found in assets folder.");
                Environment.Exit(-1);
            }

            try
            {
                nodes = JsonConvert.DeserializeObject<List<Node>>(mapText,
                    new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Populate }); //Deserializes the .json file into an array of nodes.
            }
            catch (JsonException e)
            {
                Console.WriteLine("ERROR: map.json isn't formatted correctly. \nError message:" + e.Message);
                Environment.Exit(-1);
            }
        }

        private struct DirectionSynonym
        {
            public string Direction
            {
                get;
                set;
            }
            public string[] Synonyms
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Deserializes the .json file containing all synonyms for directions.
        /// </summary>
        private static void ReadDirectionsSynonymsFromJSON(string synonymsPath)
        {
            string synonymsText = "";

            //Auxiliary array containing a collection of synonyms for each direction
            // north: {n, nrth, nor}
            // south: {s, sth, sou}
            // ...
            DirectionSynonym[] synonymsAux = { }; //Needs to be initialized

            try
            {
                //Dumps the file into a string
                synonymsText = File.ReadAllText(synonymsPath, Encoding.GetEncoding(65001)); // Encoding: UTF-8
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("ERROR: directionsSynonyms.json couldn't be found in assets folder.");
                //the lack of a synonyms file is not fatal
                return;
            }

            try
            {
                //Deserializes the .json file into a Dictionary that will be used for obtaining the synonyms for each direction
                synonymsAux = JsonConvert.DeserializeObject<DirectionSynonym[]>(synonymsText);
            }
            catch (JsonException e)
            {
                Console.WriteLine("ERROR: directionSynonyms.json isn't formatted correctly. \nError message:" + e.Message);
                Environment.Exit(-1);
            }

            //Converts the structure of synonymsAux into a more comfortable structure for finding synonyms. A dictionary<string, string>, so the new structure works like so:
            // n -> north
            // nor -> north
            // s -> south
            // ...
            foreach (var synonymList in synonymsAux)
            {
                foreach (string synonym in synonymList.Synonyms)
                {
                    directionSynonyms.Add(synonym, synonymList.Direction);
                }
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

        public static List<Node> GetNodes()
        {
            return nodes;
        }

        public static Dictionary<string, string> GetDirectionSynonyms()
        {
            return directionSynonyms;
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
            List<string> an = attackDict.Keys.ToList();
            an.Add("skip");
            return an;
        }

        public static List<string> GetAllItemActions()
        {
            List<string> actions = new List<string>();
            foreach (ObtainableItem i in itemDict.Values)
            {
                if (i.key_words == null) continue;
                foreach (string s in i.key_words.Keys)
                    actions.Add(s);
            }            
            return actions.Distinct().ToList();
        }
    }
}
