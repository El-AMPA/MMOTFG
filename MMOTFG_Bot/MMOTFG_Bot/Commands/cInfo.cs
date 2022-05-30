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
            await InformationSystem.ShowInfo(chatId, args[0]);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /info object
            if (args.Length != 1) return false;

            return true;
        }
    }
}
