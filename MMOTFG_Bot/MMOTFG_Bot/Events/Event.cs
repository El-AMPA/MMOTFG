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
    [JsonSubtypes.KnownSubType(typeof(eSendAudio), "eSendAudio")]
    [JsonSubtypes.KnownSubType(typeof(eSendImage), "eSendImage")]
    [JsonSubtypes.KnownSubType(typeof(eSendText), "eSendText")]
    [JsonSubtypes.KnownSubType(typeof(eSendImageCollection), "eSendImageCollection")]

    //Even though it's used as if it were an abstract class, it can't be abstract because in the process of deserializing into the child class,
    //it creates an instance of the parent class.
    //Thus, if it were a pure abstract class, it would just send an exception.
    class Event
    {
        public virtual async void Execute(long chatId)
        {

        }
    }
}
