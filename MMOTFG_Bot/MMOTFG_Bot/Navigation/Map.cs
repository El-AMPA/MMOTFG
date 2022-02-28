using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Navigation
{
    class Map
    {
        public enum Direction { North, West, East, South};

        Node currentNode;

        public void Navigate(long chatId, string dir)
        {
            //Move the player in the specified direction
            currentNode.OnExit(chatId, dir);
            currentNode = currentNode.GetConnectingNode(dir);
            currentNode.OnArrive(chatId, dir);
        }

        public List<Node> Nodes
        {
            get;
            set;
        }

        private Node SearchNode(string name)
        {
            foreach(Node n in Nodes)
            {
                if (n.Name == name) return n;
            }
            Console.WriteLine("ERROR: One of the requested nodes does not exist. Please check for possible spelling mistakes.");
            Environment.Exit(0);
            return null;
        }

        public void BuildMap()
        {
            Node aux;
            Console.WriteLine("Building map...");
            foreach(Node n in Nodes)
            {
                foreach(KeyValuePair<string, Node.NodeConnection> connection in n.NodeConnections)
                {
                    aux = SearchNode(connection.Value.ConnectingNode);
                    n.BuildConnection(connection.Key, n);
                    Console.WriteLine("Node " + n.Name + " leads to node " + aux.Name + " via " + connection.Key);
                }
            }
        }
    }
}