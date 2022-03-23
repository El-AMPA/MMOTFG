using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class VaquerosNormales : EquipableItem
    {
        public override void Init()
        {
            gearSlot = EQUIPMENT_SLOT.LEGS;
            statModifiers = new List<(int, StatName)>
            {
                (10, StatName.MP),
                (10, StatName.HP)
            };
        }

        public VaquerosNormales()
        {
            name = "vaqueros_normales";
        }
    }
}
