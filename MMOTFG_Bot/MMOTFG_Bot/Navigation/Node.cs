using System;
using System.Collections.Generic;
using System.Text;
using MMOTFG_Bot.Events;

namespace MMOTFG_Bot.Navigation
{
    /// <summary>
    /// Vertex of the map graph.
    /// </summary>
    class Node
    {
        /// <summary>
        /// Edge of the map graph.
        /// </summary>
        internal class NodeConnection
        {
            public string ConnectingNode
            {
                get;
                set;
            }

            public Node Node;
        }

        //TO-DO: Revisar si realmente merece la pena dejarlo como diccionario o pensar en otra estructura.
        public Dictionary<string, NodeConnection> NodeConnections
        {
            get;
            set;
        }

        public List<Event> OnArriveEvent
        {
            get;
            set;
        }

        public List<Event> OnExitEvent
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        } = "";

        public string OnInspectText
        {
            get;
            set;
        } = "";

        /// <summary>
        /// Triggers the OnExit events when leaving the node
        /// </summary>
        public void OnExit(long chatId)
        {
            if(OnExitEvent != null) foreach (Event e in OnExitEvent) e.Execute(chatId);
        }

        /// <summary>
        /// Triggers the OnArrive events when entering the node
        /// </summary>
        public void OnArrive(long chatId)
        {
            if(OnArriveEvent != null) foreach (Event e in OnArriveEvent) e.Execute(chatId);
        }

        /// <summary>
        /// Builds a new connection to a node
        /// </summary>
        public void BuildConnection(string direction, Node node)
        {
            //When deserializing the map, the Nodes are not instantiated. Each node knows that they have x connections in
            //y directions but they don't know what node they point to. They just know their name.

            NodeConnection connection;
            if(NodeConnections.TryGetValue(direction, out connection)) connection.Node = node;
        }

        /// <summary>
        /// Returns whether or not this node is connected to another node in a specified direction.
        /// </summary>
        public bool GetConnectingNode(string direction, out Node connectingNode)
        {
            NodeConnection connection;
            connectingNode = null;
            if (NodeConnections.TryGetValue(direction, out connection))
            {
                connectingNode = connection.Node;
                return true;
            }
            else return false;
        }
    }
}
