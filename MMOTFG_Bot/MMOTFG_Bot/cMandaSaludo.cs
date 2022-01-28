using System;

namespace MMOTFG_Bot
{
    class cMandaSaludo : ICommand
    {
        public string[] palabras_clave { get; set; }
        public IInteraccionable interAcc { get; set; }

        string saludo;

        public void ejecutar()
        {
            interAcc.sendToUser(0, saludo);
        }

        public cMandaSaludo(string[] palabras_clave_, IInteraccionable inter_, string saludo_)
        {
            palabras_clave = palabras_clave_;
            interAcc = inter_;
            saludo = saludo_;
        }

        public bool miraPalabras(string comando)
        {
            if (Array.Exists(palabras_clave, p => p == comando))
            {
                ejecutar();
                return true;
            }
            else return false;
        }
    }
}