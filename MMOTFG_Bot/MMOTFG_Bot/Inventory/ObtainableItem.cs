using JsonSubTypes;
using MMOTFG_Bot.Events;
using MMOTFG_Bot.Items;
using MMOTFG_Bot.Navigation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    [JsonConverter(typeof(JsonSubtypes), "ItemType")]
    [JsonSubtypes.KnownSubType(typeof(EquipableItem), "EquipableItem")]
    class ObtainableItem
    {
        public Dictionary<string, List<Event>> key_words = new Dictionary<string, List<Event>>();

        public virtual void Init() { }

        public Guid iD { get; set; }
        public string name { get; set; }
        [DefaultValue(1)]
        public int maxStackQuantity { get; set; }

        public bool UnderstandsCommand(string command)
        {
            return key_words.ContainsKey(command);
        }

        public async Task ProcessCommand(string command, string chatId, string[] args = null)
        {
            List<Event> events = key_words[command];

            await BattleSystem.LoadPlayerBattle(chatId);
            await ProgressKeeper.LoadSerializable(chatId);

            foreach (Event e in events)
            {
                e.SetUser(await BattleSystem.GetPlayer(chatId));
                await e.ExecuteEvent(chatId);
            }

            await ProgressKeeper.SaveSerializable(chatId);
            await BattleSystem.SavePlayerBattle(chatId);
        }

        public ObtainableItem()
        {
        }

        public void OnCreate()
        {
            iD = Guid.NewGuid();
        }
    }
}