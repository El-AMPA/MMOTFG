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

            public void OnArrive(long chatId)
            {
                foreach (Event e in OnArriveEvent) e.Execute(chatId);
            }

            public void OnExit(long chatId)
            {
                foreach (Event e in OnExitEvent) e.Execute(chatId);
            }

            //public Node Node
            //{
            //    get;
            //    set;
            //}
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
            NodeConnections[direction].OnExit(chatId);
        }

        public void OnArrive(long chatId, string direction)
        {
            NodeConnections[direction].OnArrive(chatId);
        }

        public void BuildConnection(string dir, NodeConnection connection)
        {

            NodeConnections.Add(dir, connection);
        }

        public Node GetConnectingNode(string direction)
        {
            //return NodeConnections[direction].Node;
            return null;
        }
    }
}
