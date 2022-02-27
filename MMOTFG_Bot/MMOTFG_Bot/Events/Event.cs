using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Newtonsoft.Json;
using JsonSubTypes;

namespace MMOTFG_Bot.Events
{
    [JsonConverter(typeof(JsonSubtypes), "EventType")]
    [JsonSubtypes.KnownSubType(typeof(eGiveItem), "eGiveItem")]
    class Event
    {
        public virtual void Execute(long chatId)
        {
            Console.WriteLine("A");
        }
    }
}
