using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    abstract class ObtainableItem
    {
        protected List<KeyValuePair<string, Action<long, string[]>>> key_words = new List<KeyValuePair<string, Action<long, string[]>>>();

        public abstract void Init();

        public Guid iD { get; set; }
        public string name { get; set; }
        public int maxStackQuantity { get; set; }

        public bool ProcessCommand(string command, long chatId, string[] args = null)
        {
            foreach (KeyValuePair<string, Action<long, string[]>> a in key_words)
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
            maxStackQuantity = 1;
        }
    }
}