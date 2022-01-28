using System;

namespace MMOTFG_Bot
{
    class Saludador: IInteraccionable
    {

        public Saludador()
		{
		}

        public void sendToUser(int _userID, string args)
        {
            Console.WriteLine($"{_userID} recibe {args}");
        }
        //abstract void moveTowards(int userID, Direction dir);        
         
        void IInteraccionable.moveTowards(int userID, Direction dir)
        {
            throw new NotImplementedException();
        }
        
        //tambien se puede usar una clase abstracta en vez de una interfaz         
    }
}