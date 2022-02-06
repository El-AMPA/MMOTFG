using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Commands
{
    class cThrowItem : ICommand
    {
        public string[] palabras_clave =
        {
            "/throw",
            "/delete",
            "/throw_away"
        };

        internal override void Execute(string command, long chatId, string[] args = null)
        {
            InventorySystem.ThrowAwayItem(chatId, new Potion(), 1);
        }

        internal override bool IsFormattedCorrectly(string[] args)
        {
            //Format: /consume itemName nItemsToUse (Optional)
            if (args.Length < 1 || args.Length > 2) return false;

            if (args.Length == 1) return true;

            if (args[1] == "all") return true;

            int numberToUse;
            if (!Int32.TryParse(args[1], out numberToUse)) return false;
            if (numberToUse <= 0) return false;

            return true;
        }

        public override bool ContainsKeyWord(string command, long chatId, string[] args = null)
        {
            if (!IsFormattedCorrectly(args))
            {
                //TelegramCommunicator.SendText(chatId, "Correct format: " + command + " itemName nItemsToUse(Opt)"); TO-DO: Me parece un poco feo que al final todo tenga que ser asíncrono. Repensar esto.
                return false;
            }
            foreach (string p in palabras_clave)
            {
                if (command == p) Execute(command, chatId, args);
            }
            return false;
        }
    }
}
