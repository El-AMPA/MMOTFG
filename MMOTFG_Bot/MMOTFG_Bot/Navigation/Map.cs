using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using MMOTFG_Bot.Battle;

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

        /// <summary>
        /// Moves the player in the specified direction
        /// </summary>
        public async static void Navigate(long chatId, string dir)
        {
            if (BattleSystem.IsPlayerInBattle(chatId))
            {
                await TelegramCommunicator.SendText(chatId, "I can't run away from battles!");
            }
            else
            {
                //If currentNode doesn't have a connection in that direction, it doesn't move the player.
                if (currentNode.GetConnectingNode(dir, out nextNode))
                {
                    currentNode.OnExit(chatId);
                    currentNode = nextNode;
                    currentNode.OnArrive(chatId);
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
        public static void Init(string mapPath)
        {
            ReadMapFromJSON(mapPath);

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
            currentNode = nodes[0];
        }

        /// <summary>
        /// Shows the available directions from CurrentNode.
        /// </summary>
        public async static void GetDirections(long chatId)
        {
            string msg = "Available directions:";
            foreach (var connection in currentNode.NodeConnections)
            {
                msg += "\n" + connection.Key;
            }

            await TelegramCommunicator.SendText(chatId, msg);
        }

        /// <summary>
        /// Sends the 'OnInspectText' field of the current node of the player 
        /// </summary>
        public static void OnInspect(long chatId)
        {
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
        private static void ReadMapFromJSON(string path)
        {
            string mapText = ""; //Text of the entire .json file
            try
            {
                mapText = File.ReadAllText(path, Encoding.GetEncoding("iso-8859-1")); //This encoding supports spanish characters "ñ, á ..."
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
    }
}