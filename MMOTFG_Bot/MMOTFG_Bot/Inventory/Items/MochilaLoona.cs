using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class MochilaLoona : EquipableItem
    {
        public override void Init()
        {
            gearSlot = EQUIPMENT_SLOT.BACK;
            statModifiers = new List<(int, StatName)>
            {
                (20, StatName.MP),
                (15, StatName.HP)
            };
        }

        public MochilaLoona()
        {
            name = "mochila_loona";
        }
    }
}
