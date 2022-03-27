using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    class bPlayerDeath : Behaviour
    {
        public override async Task<bool> Execute(long chatId) {
            //Heal all HP and MP
            user.SetStat(StatName.HP, user.GetOriginalStat(StatName.HP));
            user.SetStat(StatName.MP, user.GetOriginalStat(StatName.MP));
            return true;
        }
    }
}
