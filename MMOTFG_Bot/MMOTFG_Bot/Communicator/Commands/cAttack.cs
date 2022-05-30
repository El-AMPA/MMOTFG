using System.Threading.Tasks;
using MMOTFG_Bot.Loader;
using MMOTFG_Bot.Battle;

namespace MMOTFG_Bot.Communicator
{
    class cAttack : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Uses a certain attack in battle. You can specify the target's name.
Use: [attack name] [target name] (optional)";
        }
        public override void SetKeywords()
        {
            //keywords are every possible attack
            key_words = JSONSystem.GetAllAttackNames().ConvertAll(x => x.ToLower()).ToArray();
            showOnHelp = "attack";
        }

        internal override async Task Execute(string command, string chatId, string[] args = null)
        {
            await BattleSystem.LoadPlayerBattle(chatId);
            Player p = await BattleSystem.GetPlayer(chatId);
            if (p.learningAttack != null)
            {
                await p.ForgetAttack(chatId, command);
            }
            else
            {
                if (args.Length == 0)
                {
                    await BattleSystem.PlayerAttack(chatId, command);
                }
                else await BattleSystem.PlayerAttack(chatId, command, args[0]);
            }
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: attackName target (optional)
            return true;
        }
    }
}
