using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    static class InformationSystem
    {
        static Dictionary<string, string> information = new Dictionary<string, string>
        {
            {"manuela", "Una avispa poco amigable pero poco peligrosa."},
            {"tortazo", "Fuerza: 1.5, Coste de MP: 0" }
        };

        public static async void showInfo(long chatId, string key)
        {
            if (information.ContainsKey(key))
            {
                await TelegramCommunicator.SendText(chatId, information[key]);
            }
        }
    }
}
