using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class BeastSlayingBoots : EquipableItem
    {
        public override void Init()
        {
            gearSlot = EQUIPMENT_SLOT.FEET;
            statModifiers = new List<(int, StatName)>
            {
                (30, StatName.ATK),
                (100, StatName.HP)
            };
        }

        public BeastSlayingBoots()
        {
            name = "beast_slaying_boots";
        }
    }
}
