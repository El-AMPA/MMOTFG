using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class HealthPotion : ObtainableItem
    {
        public HealthPotion()
        {
            name = "HealthPotion";
            maxStackQuantity = 5;
            iD = Guid.NewGuid();
        }
    }
}
