﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    static class InformationSystem
    {
        static Dictionary<string, string> information = new Dictionary<string, string>
        {
            {"manuela", "Una avispa poco amigable pero poco peligrosa."},
            {"tortazo", "Fuerza: 1.5, Coste de MP: 0" },
            {"overkill", "Fuerza: 100, Coste de MP: 100"},
            {"peter", "Un ser misterioso. Ten cuidado con dejarle con poca vida..."},
            {"mp", "El coste de maná para usar determinado ataque. El ataque básico siempre cuesta 0."},
            {"/eat", "Comando que se usa para ingerir alimentos. No comas bebidas!!!1!"}
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