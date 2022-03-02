using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Events
{
    /// <summary>
    /// Sends a collection of images to the user.
    /// </summary>
    class eSendImageCollection : Event
    {
        public string[] ImagesNames
        {
            get;
            set;
        }

        public async override void Execute(long chatId)
        {
            await TelegramCommunicator.SendImageCollection(chatId, ImagesNames);
        }
    }
}
