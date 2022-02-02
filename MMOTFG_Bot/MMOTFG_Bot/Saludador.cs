using System;

namespace MMOTFG_Bot
{
    class Saludador
    {

        public Saludador()
		{
		}

        public void sendToUser(int _userID, string args)
        {
            Console.WriteLine($"{_userID} recibe {args}");
        }
       
    }
}