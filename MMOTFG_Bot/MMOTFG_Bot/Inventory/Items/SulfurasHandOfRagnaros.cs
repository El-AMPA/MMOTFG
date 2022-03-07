using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class SulfurasHandOfRagnaros : EquipableItem
    {
        public override void Init()
        {
            gearSlot = EQUIPMENT_SLOT.WEAPON;
            statModifiers = new List<(int, StatName)>
            {
                (15, StatName.ATK),
                (-2, StatName.MP)
            };
        }

        public SulfurasHandOfRagnaros()
        {
            name = "sulfuras";
        }
    }
}
