using JsonSubTypes;
using MMOTFG_Bot.Items;
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
        public List<KeyValuePair<string, Behaviour>> key_words = new List<KeyValuePair<string, Behaviour>>();

        public virtual void Init() { }

        public Guid iD { get; set; }
        public string name { get; set; }
        [DefaultValue(1)]
        public int maxStackQuantity { get; set; }

        public bool UnderstandsCommand(string command)
        {
            foreach (KeyValuePair<string, Behaviour> a in key_words)
            {
                if (a.Key == command)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> ProcessCommand(string command, string chatId, string[] args = null)
        {
            foreach (KeyValuePair<string, Behaviour> a in key_words)
            {
                if (a.Key == command)
                {
                    await a.Value.Execute(chatId, BattleSystem.player);
                    if (a.Value.message != null) await Program.Communicator.SendText(chatId, a.Value.message);
                    await BattleSystem.SavePlayerBattle(chatId);
                    return true;
                }
            }
            return false;
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