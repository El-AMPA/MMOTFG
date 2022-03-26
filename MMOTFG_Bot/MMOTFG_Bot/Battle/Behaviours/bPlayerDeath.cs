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
            user.setStat(StatName.HP, user.getOriginalStat(StatName.HP));
            user.setStat(StatName.MP, user.getOriginalStat(StatName.MP));
            return true;
        }
    }
}
