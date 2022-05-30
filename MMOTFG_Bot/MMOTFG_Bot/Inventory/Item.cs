using System;
using System.ComponentModel;
using JsonSubTypes;
using Newtonsoft.Json;

namespace MMOTFG_Bot.Inventory
{
    [JsonConverter(typeof(JsonSubtypes), "ItemType")]
    [JsonSubtypes.KnownSubType(typeof(ConsumableItem), "ConsumableItem")]
    [JsonSubtypes.KnownSubType(typeof(EquipableItem), "EquipableItem")]
    class Item
    {
        public Guid iD { get; set; }
        public string name { get; set; }
        [DefaultValue(1)]
        public int maxStackQuantity { get; set; }
        public void OnCreate()
        {
            iD = Guid.NewGuid();
        }
    }
}
