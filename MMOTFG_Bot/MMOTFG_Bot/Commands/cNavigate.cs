using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using MMOTFG_Bot.Navigation;

namespace MMOTFG_Bot.Commands
{
    /// <summary>
    /// Moves the player in the specified direction
    /// </summary>
    class cNavigate : ICommand
    {
        public override void SetDescription()
        {
            commandDescription = @"Moves the player in the given direction. For all available directions, use /directions
Use: go [direccion]";
        }
        public override void SetKeywords()
        {
            key_words = new string[]{
                "go",
                "g"
            };
        }

        internal override async Task Execute(string command, string chatId, string[] args = null)
        {
            bool inParty = await PartySystem.IsInParty(chatId);
            if (!inParty) await Map.Navigate(chatId, args[0]);
            else
            {
                bool leader = await PartySystem.IsLeader(chatId);
                if (!leader) await TelegramCommunicator.SendText(chatId, "Only the party leader can move.");
                else
                {
                    string code = await PartySystem.GetPartyCode(chatId);
                    await Map.Navigate(chatId, args[0], code);
                }
            }
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            if (args.Length != 1) return false;

            return true;
        }
    }
}
