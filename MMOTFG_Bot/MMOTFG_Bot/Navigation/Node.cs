using System;
using System.Collections.Generic;
using System.Text;
using MMOTFG_Bot.Events;

namespace MMOTFG_Bot.Navigation
{
    class Node
    {
        public class NodeConnection
        {
            public string ConnectingNode
            {
                get;
                set;
            }

            public Node Node;
        }

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

        public void OnExit(long chatId)
        {
            if(OnExitEvent != null) foreach (Event e in OnExitEvent) e.Execute(chatId);
        }

        public void OnArrive(long chatId)
        {
            if(OnArriveEvent != null) foreach (Event e in OnArriveEvent) e.Execute(chatId);
        }

        public void BuildConnection(string direction, Node node)
        {
            NodeConnection connection;
            if(NodeConnections.TryGetValue(direction, out connection)) connection.Node = node;
        }

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
