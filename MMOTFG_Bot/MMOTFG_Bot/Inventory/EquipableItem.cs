using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class EquipableItem : ObtainableItem
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public EQUIPMENT_SLOT gearSlot
        {
            get;
            set;
        }

        public Dictionary<StatName, int> statModifiers
        {
            get;
            set;
        }

        public void OnEquip(long chatId, string[] args = null)
        {
            foreach (var stat in statModifiers)
            {
                BattleSystem.player.AddToStat(stat.Key, stat.Value, changeMax: true, permanent: true);
            }
        }

        public void OnUnequip(long chatId, string[] args = null)
        {
            foreach (var stat in statModifiers)
            {
                BattleSystem.player.AddToStat(stat.Key, -stat.Value, changeMax: true, permanent: true);
            }
        }
    }
}
