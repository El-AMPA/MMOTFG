using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class ManaPotion : ObtainableItem
    {
        public ManaPotion()
        {
            name = "ManaPotion";
            maxStackQuantity = 3;
            iD = Guid.NewGuid();
        }
    }
}
