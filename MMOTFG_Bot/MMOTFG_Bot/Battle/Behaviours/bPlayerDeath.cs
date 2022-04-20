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
        public override async Task<bool> Execute(string chatId, Battler user) {
            //Set player as dead
            return true;
        }
    }
}