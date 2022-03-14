using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class JoyConConDrift : EquipableItem
    {
        public override void Init()
        {
            gearSlot = EQUIPMENT_SLOT.WEAPON;
            statModifiers = new List<(int, StatName)>
            {
                (10, StatName.ATK),
            };
        }

        public JoyConConDrift()
        {
            name = "joycon_con_drift";
        }
    }
}
