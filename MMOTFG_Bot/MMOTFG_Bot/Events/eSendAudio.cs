using System.Threading.Tasks;
using MMOTFG_Bot.Communicator;

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

        public override async Task Execute(string chatId)
        {
            await TelegramCommunicator.SendAudio(chatId, AudioName, Description);
        }
    }
}
