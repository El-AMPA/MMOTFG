using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using MMOTFG_Bot.Battle;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Navigation
{
    /// <summary>
    /// Map that stores the world of the whole game.
    /// </summary>
    class Map
    {
        private static Node currentNode;
        private static Node nextNode;
        private static List<Node> nodes;
        private static Dictionary<string, string> directionSynonyms = new Dictionary<string, string>();
        private static Node startingNode;

        /// <summary>
        /// Moves the player in the specified direction
        /// </summary>
        public async static void Navigate(long chatId, string dir)
        {
            await LoadPlayerPosition(chatId);

            if (await BattleSystem.IsPlayerInBattle(chatId))
            {
                await TelegramCommunicator.SendText(chatId, "I can't run away from battles!");
            }
            else
            {
                dir = GetSynonymDirection(dir);
                //If currentNode doesn't have a connection in that direction, it doesn't move the player.
                if (currentNode.GetConnectingNode(dir, out nextNode))
                {
                    currentNode.OnExit(chatId);
                    currentNode = nextNode;
                    currentNode.OnArrive(chatId);
                    await SavePlayerPosition(chatId);
                }
                else
                {
                    await TelegramCommunicator.SendText(chatId, "Can't move in that direction");
                }
            }
        }

        /// <summary>
        /// Builds the map reading it from the file specified by mapPath.
        /// </summary>
        public static void Init(string mapPath, string synonymsPath = "")
        {
            ReadMapFromJSON(mapPath);
            if(synonymsPath != "")ReadDirectionsSynonymsFromJSON(synonymsPath);

            //Connecting the map together.
            //When deserializing the map and creating a new Node, not every Node that is connected to the new node is instanced. Each node knows that they have x connections in
            //y directions but they don't know what node instance they point to. They just know their name. 
            //That's why after deserialzing, we need to complete the connections one by one.
            Node aux;
            Console.WriteLine("Building map...");
            foreach (Node n in nodes)
            {
                foreach (KeyValuePair<string, Node.NodeConnection> connection in n.NodeConnections)
                {
                    aux = SearchNode(connection.Value.ConnectingNode);
                    n.BuildConnection(connection.Key, aux);
                    Console.WriteLine("Node " + n.Name + " leads to node " + aux.Name + " via " + connection.Key);
                }
            }
            startingNode = nodes[0];
        }

        /// <summary>
        /// Shows the available directions from CurrentNode.
        /// </summary>
        public async static void GetDirections(long chatId)
        {
            await LoadPlayerPosition(chatId);

            string msg = "Available directions:";
            foreach (var connection in currentNode.NodeConnections)
            {
                msg += "\n/go_" + connection.Key;
            }

            await TelegramCommunicator.SendText(chatId, msg);
        }

        /// <summary>
        /// Sends the 'OnInspectText' field of the current node of the player 
        /// </summary>
        public async static void OnInspect(long chatId)
        {
            await LoadPlayerPosition(chatId);
            currentNode.OnInspect(chatId);
        }

        /// <summary>
        /// Searches a node in the Nodes list by a given name
        /// </summary>
        private static Node SearchNode(string name)
        {
            foreach(Node n in nodes)
            {
                if (n.Name == name) return n;
            }
            Console.WriteLine("ERROR: One of the requested nodes does not exist. Please check for possible spelling mistakes.");
            Environment.Exit(0);
            return null;
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
            catch (FileNotFoundException e)
            {
                Console.WriteLine("ERROR: map.json couldn't be found in assets folder.");
                Environment.Exit(-1);
            }

            try
            {
                nodes = JsonConvert.DeserializeObject<List<Node>>(mapText); //Deserializes the .json file into an array of nodes.
            }
            catch (JsonException e)
            {
                Console.WriteLine("ERROR: map.json isn't formatted correctly. \nError message:" + e.Message);
                Environment.Exit(-1);
            }
        }

        private static string GetSynonymDirection(string dir)
        {
            string synonym;
            if (directionSynonyms.TryGetValue(dir, out synonym)) return synonym;
            else return dir;
        }

        private static void ReadDirectionsSynonymsFromJSON(string synonymsPath)
        {
            string synonymsText = "";
            ValueTuple<string, string[]>[] synonymsAux = { }; //Needs to be initialized

            try
            {
                synonymsText = File.ReadAllText(synonymsPath, Encoding.GetEncoding(65001)); // Encoding: UTF-8
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("ERROR: directionsSynonyms.json couldn't be found in assets folder.");
                Environment.Exit(-1);
            }

            try
            {
                synonymsAux = JsonConvert.DeserializeObject<ValueTuple<string, string[]>[]>(synonymsText); //Deserializes the .json file into a Dictionary that will be used for obtaining the synonyms for each direction
            }
            catch (JsonException e)
            {
                Console.WriteLine("ERROR: directionSynonyms.json isn't formatted correctly. \nError message:" + e.Message);
                Environment.Exit(-1);
            }

            foreach(var synonymList in synonymsAux)
            {
                foreach(string synonym in synonymList.Item2)
                {
                    directionSynonyms.Add(synonym, synonymList.Item1);
                }
            }
        }

        /// <summary>
        /// Saves the actual node of the given player to the database
        /// </summary>
        public static async Task SavePlayerPosition(long chatId)
		{
            Dictionary<string, object> update = new Dictionary<string, object>();

            update.Add(DbConstants.PLAYER_FIELD_ACTUAL_NODE, currentNode.Name);

            await DatabaseManager.ModifyDocumentFromCollection(update, chatId.ToString(), DbConstants.COLLEC_DEBUG);
        }

        /// <summary>
        /// Loads the actual node of the given player from the database
        /// </summary>
        public static async Task LoadPlayerPosition(long chatId)
        {
            Dictionary<string, object> player = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID,
                chatId.ToString(), DbConstants.COLLEC_DEBUG);

            string currNodeName = player[DbConstants.PLAYER_FIELD_ACTUAL_NODE].ToString();

            currentNode = nodes.Find(x => (x.Name == currNodeName));
        }

        /// <summary>
        /// Creates the node field in the player document in the database
        /// </summary>
        public static async Task CreatePlayerPosition(long chatId)
		{
            Dictionary<string, object> update = new Dictionary<string, object>();

            update.Add(DbConstants.PLAYER_FIELD_ACTUAL_NODE, startingNode.Name);

            await DatabaseManager.ModifyDocumentFromCollection(update, chatId.ToString(), DbConstants.COLLEC_DEBUG);
        }
    }
}