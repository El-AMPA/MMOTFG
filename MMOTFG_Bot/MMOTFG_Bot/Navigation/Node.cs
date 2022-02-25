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

            public void OnArrive(long chatId)
            {
                foreach (Event e in OnArriveEvent) e.Execute(chatId);
            }

            public void OnExit(long chatId)
            {
                foreach (Event e in OnExitEvent) e.Execute(chatId);
            }

            public Node Node
            {
                get;
                set;
            }
        }

        public List<NodeConnection> NodeConnections
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public void OnExit(long chatId, Map.Direction direction)
        {
            NodeConnections[(int)direction].OnExit(chatId);
        }

        public void OnArrive(long chatId, Map.Direction direction)
        {
            NodeConnections[(int)direction].OnArrive(chatId);
        }

        public Node GetConnectingNode(Map.Direction direction)
        {
            return NodeConnections[(int)direction].Node;
        }
    }
}
