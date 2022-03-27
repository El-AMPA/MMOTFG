using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Events
{
    /// <summary>
    /// Sends an audio file with an optional description to the user.
    /// </summary>
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

        public override async Task Execute(long chatId)
        {
            await TelegramCommunicator.SendAudio(chatId, AudioName, Description);
        }
    }
}
