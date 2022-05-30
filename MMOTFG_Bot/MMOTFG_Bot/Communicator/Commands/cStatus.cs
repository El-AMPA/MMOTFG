using System.Threading.Tasks;
using MMOTFG_Bot.Inventory;
using MMOTFG_Bot.Battle;
using MMOTFG_Bot.Multiplayer;

namespace MMOTFG_Bot.Communicator
{
    class cStatus : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Lists the stats and gear of a certain character.
When used by itself, it will display the player's status. Can also be used to know the status of another player or enemy.
use: status [character name]";
        }
        public override void SetKeywords()
        {
            key_words = new string[] {
                "status",
                "s"
            };
        }

        internal override async Task Execute(string command, string chatId, string[] args = null)
        {
            if (args.Length == 0)
            {
                await BattleSystem.ShowStatus(chatId);
                await InventorySystem.ShowGear(chatId);
            }
            else
            {
                //If it's someone from your party the game shows their gear too
                string friendId = await PartySystem.GetFriendId(chatId, args[0]);
                await BattleSystem.ShowStatus(chatId, args[0]);
                if (friendId != null) await InventorySystem.ShowGear(chatId, friendId);
            }
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /status enemy (optional)
            if (args.Length > 1) return false;

            return true;
        }
    }
}
