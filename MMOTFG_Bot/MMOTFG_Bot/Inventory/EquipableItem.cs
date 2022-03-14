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
                if ((stat.Item2 == StatName.HP || stat.Item2 == StatName.MP))
                {
                    float currentPercent = BattleSystem.player.stats[(int)stat.Item2] / BattleSystem.player.originalStats[(int)stat.Item2];
                    BattleSystem.player.originalStats[(int)stat.Item2] += stat.Item1;
                    BattleSystem.player.stats[(int)stat.Item2] = (int)(BattleSystem.player.originalStats[(int)stat.Item2] * currentPercent);
                }
                else
                {
                    BattleSystem.player.stats[(int)stat.Item2] += stat.Item1;
                    BattleSystem.player.originalStats[(int)stat.Item2] += stat.Item1;
                }
            }
        }

        public void OnUnequip(long chatId, string[] args = null)
        {
            foreach(var stat in statModifiers)
            {
                if ((stat.Item2 == StatName.HP || stat.Item2 == StatName.MP))
                {
                    float currentPercent = BattleSystem.player.stats[(int)stat.Item2] / BattleSystem.player.originalStats[(int)stat.Item2];
                    BattleSystem.player.originalStats[(int)stat.Item2] -= stat.Item1;
                    BattleSystem.player.stats[(int)stat.Item2] = (int)(BattleSystem.player.originalStats[(int)stat.Item2] * currentPercent);
                }
                else
                {
                    BattleSystem.player.stats[(int)stat.Item2] -= stat.Item1;
                    BattleSystem.player.originalStats[(int)stat.Item2] -= stat.Item1;
                }
            }
        }
    }
}
