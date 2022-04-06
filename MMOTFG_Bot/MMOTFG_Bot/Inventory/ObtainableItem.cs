using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    abstract class ObtainableItem
    {
        protected List<KeyValuePair<string, Action<string, string[]>>> key_words = new List<KeyValuePair<string, Action<string, string[]>>>();

        public abstract void Init();

        public Guid iD { get; set; }
        public string name { get; set; }
        public int maxStackQuantity { get; set; }

        public bool UnderstandsCommand(string command)
        {
            foreach (KeyValuePair<string, Action<string, string[]>> a in key_words)
            {
                if (a.Key == command)
                {
                    return true;
                }
            }
            return false;
        }

        public bool ProcessCommand(string command, string chatId, string[] args = null)
        {
            foreach (KeyValuePair<string, Action<string, string[]>> a in key_words)
            {
                if (a.Key == command)
                {
                    a.Value(chatId, args);
                    return true;
                }
            }
            return false;
        }

        public ObtainableItem()
        {
            iD = Guid.NewGuid();
            maxStackQuantity = 1;
        }
    }
}