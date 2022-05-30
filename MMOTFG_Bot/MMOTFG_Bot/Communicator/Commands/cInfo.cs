using System.Threading.Tasks;
using MMOTFG_Bot.Loader;
using MMOTFG_Bot.Inventory;
using MMOTFG_Bot.Battle;

namespace MMOTFG_Bot.Communicator
{
    class cInfo : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Gives information on a certain item or attack
Uso: info [name]";
        }
        public override void SetKeywords()
        {
            key_words = new string[] {
                "info"
            };
        }

        internal override async Task Execute(string command, string chatId, string[] args = null)
        {
            string name = string.Join(" ", args);
            Item i = JSONSystem.GetItem(name);
            if (i != null) await TelegramCommunicator.SendText(chatId, i.GetInformation());
            else
            {
                Attack a = JSONSystem.GetAttack(name);
                if (a != null) await TelegramCommunicator.SendText(chatId, a.GetInformation());
                else await TelegramCommunicator.SendText(chatId, "Name not recognized");
            }
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /info [name]
            if (args.Length == 0) return false;

            return true;
        }
    }
}
