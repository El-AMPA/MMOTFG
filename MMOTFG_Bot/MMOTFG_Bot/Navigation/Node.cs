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

            public string ConnectingNode
            {
                get;
                set;
            }

            private Node Node;

            public void OnArrive(long chatId)
            {
                foreach (Event e in OnArriveEvent) e.Execute(chatId);
            }

            public void OnExit(long chatId)
            {
                foreach (Event e in OnExitEvent) e.Execute(chatId);
            }

            public void SetNode(Node node)
            {
                Node = node;
            }
        }

        public Dictionary<string, NodeConnection> NodeConnections
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public void OnExit(long chatId, string direction)
        {
            NodeConnection connection = GetConnectionFromDirection(direction);
            if (connection != null) connection.OnExit(chatId);
        }

        public void OnArrive(long chatId, string direction)
        {
            NodeConnection connection = GetConnectionFromDirection(direction);
            if (connection != null) connection.OnArrive(chatId);
        }

        public void BuildConnection(string direction, Node node)
        {
            NodeConnection connection = GetConnectionFromDirection(direction);
            if (connection != null) connection.SetNode(node);
        }

        private NodeConnection GetConnectionFromDirection(string direction)
        {
            return NodeConnections[direction];
        }

        public Node GetConnectingNode(string direction)
        {
            //return NodeConnections[direction].Node;
            return null;
        }
    }
}
