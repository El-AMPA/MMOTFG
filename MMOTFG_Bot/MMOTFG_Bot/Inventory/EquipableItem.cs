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

        public virtual void OnEquip(long chatId, string[] args = null)
        {
            foreach (var stat in statModifiers)
            {
                BattleSystem.player.AddToStat(stat.Item2, stat.Item1, changeMax: true, permanent: true);
            }
        }

        public virtual void OnUnequip(long chatId, string[] args = null)
        {
            foreach (var stat in statModifiers)
            {
                BattleSystem.player.AddToStat(stat.Item2, -stat.Item1, changeMax: true, permanent: true);
            }
        }
    }
}
