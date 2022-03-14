using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class ZapatillasGastadas : EquipableItem
    {
        public override void Init()
        {
            gearSlot = EQUIPMENT_SLOT.FEET;
            statModifiers = new List<(int, StatName)>
            {
                (10, StatName.MP)
            };
        }

        public ZapatillasGastadas()
        {
            name = "zapatillas_gastadas";
        }
    }
}
