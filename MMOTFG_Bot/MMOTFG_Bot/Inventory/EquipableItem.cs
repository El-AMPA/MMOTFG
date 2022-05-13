﻿using MMOTFG_Bot.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using MMOTFG_Bot.Battle;

namespace MMOTFG_Bot.Inventory
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

        public List<Event> onEquipEvents;
        public List<Event> onUnequipEvents;

        public virtual async void OnEquip(string chatId, string[] args = null)
        {
            foreach (var stat in statModifiers)
            {
                BattleSystem.player.AddToStat(stat.Key, stat.Value, changeMax: true, permanent: true);
            }

            if(onEquipEvents != null)
                foreach (Event e in onEquipEvents) await e.Execute(chatId);
        }

        public virtual async void OnUnequip(string chatId, string[] args = null)
        {
            foreach (var stat in statModifiers)
            {
                BattleSystem.player.AddToStat(stat.Key, -stat.Value, changeMax: true, permanent: true);
            }

            if (onUnequipEvents != null)
                foreach (Event e in onUnequipEvents) await e.Execute(chatId);
        }
    }
}
