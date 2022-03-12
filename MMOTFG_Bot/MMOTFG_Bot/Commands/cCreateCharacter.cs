using System;
using System.Collections.Generic;
using System.Text;
using MMOTFG_Bot.Navigation;

namespace MMOTFG_Bot.Commands
{
    /// <summary>
    /// Shows the available directions from a given node.
    /// </summary>
    class cCreateCharacter : ICommand
    {
        public override void SetKeywords()
        {
            key_words = new string[] {
                "/create"
            };
        }

        async internal override void Execute(string command, long chatId, string[] args = null)
        {

            //await InventorySystem.saveToDatabase(chatId);

			Dictionary<string, object> dict = new Dictionary<string, object>
			{               

                { DbConstants.PLAYER_FIELD_NAME , args[0]},
				{ DbConstants.PLAYER_FIELD_TELEGRAM_ID, chatId.ToString()}
			};

			bool created = await DatabaseManager.addDocumentToCollection(dict, chatId.ToString(), DbConstants.COLLEC_DEBUG);

			if (!created) await TelegramCommunicator.SendText(chatId, "That name is already in use");
		}

        internal override bool IsFormattedCorrectly(string[] args)
        {
            if (args.Length == 1) return true;
            return false;
        }
    }
}

