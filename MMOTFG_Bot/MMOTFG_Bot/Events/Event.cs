using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot
{
    abstract class Event
    {
        public abstract void Execute(long chatId);
    }
}
