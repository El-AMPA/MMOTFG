using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Events
{
    class eSendAudio : Event
    {
        public string Description
        {
            get;
            set;
        }

        public string AudioName
        {
            get;
            set;
        }

        public async override void Execute(long chatId)
        {
            await TelegramCommunicator.SendAudio(chatId, AudioName, Description);
        }
    }
}
