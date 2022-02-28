using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Events
{
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
