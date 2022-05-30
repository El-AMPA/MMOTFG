using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using MMOTFG_Bot.Events;
using MMOTFG_Bot.Navigation;
using MMOTFG_Bot.Battle;

namespace MMOTFG_Bot.Inventory
{
    class EquipableItem : Item
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
                (BattleSystem.GetPlayer(chatId)).AddToStat(stat.Key, stat.Value, changeMax: true, permanent: true);
            }

            if(onEquipEvents != null)
            {
                await ProgressKeeper.LoadSerializable(chatId);

                foreach (Event ev in onEquipEvents)
                {
                    await ev.ExecuteEvent(chatId);
                }

                await ProgressKeeper.SaveSerializable(chatId);
            }
        }

        public virtual async void OnUnequip(string chatId, string[] args = null)
        {
            foreach (var stat in statModifiers)
            {
                (BattleSystem.GetPlayer(chatId)).AddToStat(stat.Key, -stat.Value, changeMax: true, permanent: true);
            }

            if (onUnequipEvents != null)
            {
                await ProgressKeeper.LoadSerializable(chatId);

                foreach (Event ev in onUnequipEvents)
                {
                    await ev.ExecuteEvent(chatId);
                }

                await ProgressKeeper.SaveSerializable(chatId);
            }
        }
    }
}
