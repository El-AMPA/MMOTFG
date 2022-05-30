using System.Threading.Tasks;
using MMOTFG_Bot.Communicator;

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

        public override async Task Execute(string chatId)
        {
            await TelegramCommunicator.SendText(chatId, Description);
        }
    }
}
