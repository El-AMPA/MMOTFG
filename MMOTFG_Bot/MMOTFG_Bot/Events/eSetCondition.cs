using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Events
{
    /// <summary>
    /// Sends a text message to the user.
    /// </summary>
    class eSetCondition : Event
    {
        public string Name
        {
            get;
            set;
        }

        public bool SetAs
        {
            get;
            set;
        }

        public async override Task Execute(long chatId)
        {
            await TelegramCommunicator.SendText(chatId, "Beep boop i'm changing your condición");
        }
    }
}
