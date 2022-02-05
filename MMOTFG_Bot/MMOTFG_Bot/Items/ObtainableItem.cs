using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    abstract class ObtainableItem
    {
        public Guid iD { get; set; }
        public string name { get; set; }
        public int maxStackQuantity { get; set; }

        public ObtainableItem()
        {
            maxStackQuantity = 1;
        }
    }
}