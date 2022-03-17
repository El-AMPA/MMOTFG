using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MMOTFG_Bot.Battle.Enemies
{
    static class EnemySystem
    {
        //static Dictionary<string, Enemy> enemyDict = new Dictionary<string, Enemy>
        //{
        //    {"Manuela", new Manuela()},
        //    {"Peter", new Peter()}
        //};

        //public static Enemy getEnemy(string name)
        //{
        //    if (enemyDict.ContainsKey(name))
        //    {
        //        return enemyDict[name];
        //    }
        //    else return new Error();
        //}

        private static List<Enemy> enemies;
        public static void Init(string enemyPath)
        {
            ReadEnemiesFromJSON(enemyPath);
        }

        /// <summary>
        /// Deserializes the .json file specified by path and constructs the map.
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
                Console.WriteLine("ERROR: map.json couldn't be found in assets folder.");
                Environment.Exit(-1);
            }

            try
            {
                enemies = JsonConvert.DeserializeObject<List<Enemy>>(enemyText); //Deserializes the .json file into an array of nodes.
                foreach (Enemy e in enemies) e.onCreate();
            }
            catch (JsonException e)
            {
                Console.WriteLine("ERROR: map.json isn't formatted correctly. \nError message:" + e.Message);
                Environment.Exit(-1);
            }
        }

        public static Enemy getEnemy(string name)
        {
            Enemy e = enemies.Find(x => x.name == name);
            if (e != null)
            {
                return e;
            }
            else return new Error();
        }
    }
}
