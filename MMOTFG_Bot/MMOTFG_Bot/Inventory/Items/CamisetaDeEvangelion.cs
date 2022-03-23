using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class CamisetaDeEvangelion : EquipableItem
    {
        public override void Init()
        {
            gearSlot = EQUIPMENT_SLOT.CHEST;
            statModifiers = new List<(int, StatName)>
            {
                (-2, StatName.ATK),
                (10, StatName.MP)
            };
        }

        public CamisetaDeEvangelion()
        {
            name = "camiseta_de_evangelion";
        }
    }
}
