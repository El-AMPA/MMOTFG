using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Events
{
    /// <summary>
    /// Sends a single image with an optional description to the user.
    /// </summary>
    class eSendImage : Event
    {
        public string Descrption
        {
            get;
            set;
        } = "";

        public string ImageName
        {
            get;
            set;
        }

        public async override void Execute(long chatId)
        {
            await TelegramCommunicator.SendImage(chatId, ImageName, Descrption);
        }
    }
}
