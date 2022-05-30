using System.Threading.Tasks;
using MMOTFG_Bot.Communicator;

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

        public override async Task Execute(string chatId)
        {
            await TelegramCommunicator.SendImageCollection(chatId, ImagesNames);
        }
    }
}
