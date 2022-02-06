using System;

namespace MMOTFG_Bot
{
    class cMandaSaludo : ICommand
    {
        public string[] palabras_clave { get; set; }

        string saludo;

        Saludador saludador;

        public void ejecutar()
        {
            saludador.sendToUser(0, saludo);
        }

        public cMandaSaludo(string[] palabras_clave_, Saludador saludador_, string saludo_)
        {
            palabras_clave = palabras_clave_;
            saludador = saludador_;
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