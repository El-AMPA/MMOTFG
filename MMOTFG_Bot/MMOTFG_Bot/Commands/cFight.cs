using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Commands
{
    class cFight : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Starts a battle with the specified enemy or enemies.
If in a party, can only be used by the leader.
Use: fight [enemyName] [enemyName2] [enemyName3]...";
        }

        public override void SetKeywords()
        {
            key_words = new string[] {
                "fight"
            };
        }

        internal override async Task Execute(string command, string chatId, string[] args = null)
        {
            List<Enemy> enemies = new List<Enemy>();

            for (int i = 0; i < args.Length; i++)
            {
                Enemy e = JSONSystem.GetEnemy(args[i]);
                if (e != null) enemies.Add(e);
                else await TelegramCommunicator.SendText(chatId, $"{args[i]} is an invalid enemy, will be ignored");
            }

            bool isInParty = await PartySystem.IsInParty(chatId);
            bool isLeader = await PartySystem.IsLeader(chatId);

            if (isInParty && !isLeader) {
                await TelegramCommunicator.SendText(chatId, "Only leader can start battles");
                return;
            }

            List<string> chatIds = new List<string>();

            chatIds.Add(chatId);
            if (isInParty)
            {
                string partyCode = await PartySystem.GetPartyCode(chatId);
                foreach (string id in await PartySystem.GetPartyMembers(partyCode))
                    chatIds.Add(id);
            }

            if(enemies.Count == 0) await TelegramCommunicator.SendText(chatId, "No valid enemies");

            else await BattleSystem.StartBattle(chatIds, enemies);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: fight [enemyName] [enemyName]...
            return args.Length > 0;
        }
    }
}
