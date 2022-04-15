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
        public string Name { get; set; }

        public bool SetAs { get; set; }

        public override async Task<bool> Execute(string chatId)
        {
            await ProgressKeeper.LoadSerializable(chatId);
            if (Name != null) ProgressKeeper.SetFlagAs(chatId, Name, SetAs);
            await ProgressKeeper.SaveSerializable(chatId);

            return true;
        }
    }
}
