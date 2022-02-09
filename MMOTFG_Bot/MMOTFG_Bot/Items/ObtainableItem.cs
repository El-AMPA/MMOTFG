using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    abstract class ObtainableItem
    {
        protected KeyValuePair<string, Action<long, string[]>>[] key_words;

        public abstract void Init();

        public Guid iD { get; set; }
        public string name { get; set; }
        public int maxStackQuantity { get; set; }

        public abstract bool ProcessCommand(string command, long chatId, string[] args = null);

        public ObtainableItem()
        {
            maxStackQuantity = 1;
        }
    }
}