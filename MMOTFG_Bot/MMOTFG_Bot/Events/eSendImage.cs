using System.Threading.Tasks;
using MMOTFG_Bot.Communicator;

namespace MMOTFG_Bot.Events
{
    /// <summary>
    /// Sends a single image with an optional description to the user.
    /// </summary>
    class eSendImage : Event
    {
        public string Description
        {
            get;
            set;
        }

        public string ImageName
        {
            get;
            set;
        }

        public override async Task Execute(string chatId)
        {
            await TelegramCommunicator.SendImage(chatId, ImageName, false, Description);
        }
    }
}
