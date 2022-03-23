using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class CollarDeJorge : EquipableItem
    {
        public override void Init()
        {
            gearSlot = EQUIPMENT_SLOT.TRINKET;
            statModifiers = new List<(int, StatName)>
            {
                (20, StatName.ATK),
                (-10, StatName.HP)
            };
        }

        public CollarDeJorge()
        {
            name = "collar_de_jorge";
        }
    }
}
