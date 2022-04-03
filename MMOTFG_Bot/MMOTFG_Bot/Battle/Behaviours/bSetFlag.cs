using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;
using MMOTFG_Bot.Navigation;

namespace MMOTFG_Bot
{
    class bSetFlag : Behaviour
    {
        public string SetFlagAsTrue { get; set; }

        public string SetFlagAsFalse { get; set; }

        public override async Task<bool> Execute(long chatId)
        {
            await ProgressKeeper.LoadSerializable(chatId);
            if (SetFlagAsTrue != null) ProgressKeeper.SetFlagAs(chatId, SetFlagAsTrue, true);
            else if (SetFlagAsFalse != null) ProgressKeeper.SetFlagAs(chatId, SetFlagAsFalse, false);
            await ProgressKeeper.SaveSerializable(chatId);

            return true;
        }
    }
}
