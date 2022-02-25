using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Navigation
{
    class Map
    {
        public enum Direction { North, West, East, South};

        Node currentNode;

        public void Navigate(long chatId, Direction dir)
        {
            //Move the player in the specified direction
            currentNode.OnExit(chatId, dir);
            
        }

        public List<Node> Nodes
        {
            get;
            set;
        }
    }
}