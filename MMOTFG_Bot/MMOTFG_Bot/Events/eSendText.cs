using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Events
{
    /// <summary>
    /// Sends a text message to the user.
    /// </summary>
    class eSendText : Event
    {
        public string Description
        {
            get;
            set;
        }

        public async override void Execute(long chatId)
        {
            await TelegramCommunicator.SendText(chatId, Description);
        }
    }
}
