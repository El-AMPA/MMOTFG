using System.Threading.Tasks;

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
            ObtainableItem i = JSONSystem.GetItem(name);
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
