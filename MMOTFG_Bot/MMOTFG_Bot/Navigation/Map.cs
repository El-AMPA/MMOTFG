using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace MMOTFG_Bot.Navigation
{
    class Map
    {
        private static Node currentNode;
        private static Node nextNode;

        public async static void Navigate(long chatId, string dir)
        {
            //Move the player in the specified direction
            if(currentNode.GetConnectingNode(dir, out nextNode))
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

        private static List<Node> nodes;

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

        private static void ReadMapFromJSON(string path)
        {
            string mapText = "";
            try
            {
                mapText = File.ReadAllText(path, Encoding.GetEncoding("iso-8859-1"));
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine("ERROR: map.json couldn't be found in assets folder.");
                Environment.Exit(-1);
            }

            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.Culture = new System.Globalization.CultureInfo("es-ES", false);
                
                nodes = JsonConvert.DeserializeObject<List<Node>>(mapText);
            }
            catch (JsonException e)
            {
                Console.WriteLine("ERROR: map.json isn't formatted correctly. \nError message:" + e.Message);
                Environment.Exit(-1);
            }
        }

        public static void BuildMap(string mapPath)
        {
            ReadMapFromJSON(mapPath);

            //Connecting the map together.
            Node aux;
            Console.WriteLine("Building map...");
            foreach(Node n in nodes)
            {
                foreach(KeyValuePair<string, Node.NodeConnection> connection in n.NodeConnections)
                {
                    aux = SearchNode(connection.Value.ConnectingNode);
                    n.BuildConnection(connection.Key, aux);
                    Console.WriteLine("Node " + n.Name + " leads to node " + aux.Name + " via " + connection.Key);
                }
            }

            currentNode = nodes[0];
        }

        public async static void GetDirections(long chatId)
        {
            string msg = "Available directions:";
            foreach(var connection in currentNode.NodeConnections)
            {
                msg += "\n" + connection.Key;
            }

            await TelegramCommunicator.SendText(chatId, msg);
        }

        public async static void OnInspect(long chatId)
        {
            if(currentNode.OnInspectText != "")
            {
                await TelegramCommunicator.SendText(chatId, currentNode.OnInspectText);
            }
            else await TelegramCommunicator.SendText(chatId, "There is nothing of interest around here");
        }
    }
}