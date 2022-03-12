using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class ThunderfuryBleesedBladeOfTheWindseeker : EquipableItem
    {
        public override void Init()
        {
            gearSlot = EQUIPMENT_SLOT.WEAPON;
            statModifiers = new List<(int, StatName)>
            {
                (10, StatName.ATK),
                (-5, StatName.HP)
            };
        }

        public ThunderfuryBleesedBladeOfTheWindseeker()
        {
            name = "tfury";
        }
    }
}
