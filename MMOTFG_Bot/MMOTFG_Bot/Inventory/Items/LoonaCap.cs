using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class LoonaCap : EquipableItem
    {
        public override void Init()
        {
            gearSlot = EQUIPMENT_SLOT.HEAD;
            statModifiers = new List<(int, StatName)>
            {
                (20, StatName.MP),
                (15, StatName.HP)
            };
        }

        public LoonaCap()
        {
            name = "loonaCap";
        }
    }
}
