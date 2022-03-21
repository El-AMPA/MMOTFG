using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    abstract class EquipableItem : ObtainableItem
    {
        public EQUIPMENT_SLOT gearSlot
        {
            get;
            set;
        }

        public List<(int, StatName)> statModifiers
        {
            get;
            protected set;
        }

        public void OnEquip(long chatId, string[] args = null)
        {
            foreach (var stat in statModifiers)
            {
                BattleSystem.player.changeOriginalStat(stat.Item2, stat.Item1);
            }
        }

        public void OnUnequip(long chatId, string[] args = null)
        {
            foreach (var stat in statModifiers)
            {
                BattleSystem.player.changeOriginalStat(stat.Item2, -stat.Item1);
            }
        }
    }
}
