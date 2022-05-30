using System.Collections.Generic;
using System.Threading.Tasks;
using MMOTFG_Bot.Events;
using MMOTFG_Bot.Navigation;
using MMOTFG_Bot.Battle;

namespace MMOTFG_Bot.Inventory
{
    class ConsumableItem: Item
    {
        public Dictionary<string, List<Event>> key_words = new Dictionary<string, List<Event>>();

        public virtual void Init() { }

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

        public ConsumableItem()
        {
        }
    }
}