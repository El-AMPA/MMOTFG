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

        public void foo(long chatid)
        {
            Nodes[0].OnArrive(chatid, "north");
        }
    }
}