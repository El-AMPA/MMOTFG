using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class Tatirana : EquipableItem
    {
        public override void Init()
        {
            gearSlot = EQUIPMENT_SLOT.WEAPON;
            statModifiers = new List<(int, StatName)>
            {
                (15, StatName.ATK),
                (15, StatName.MP)
            };
        }

        public Tatirana()
        {
            name = "tatirana";
        }
    }
}
