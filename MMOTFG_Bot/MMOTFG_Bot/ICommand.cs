namespace MMOTFG_Bot
{
    interface ICommand
    {
        string[] palabras_clave { get; set; }
        IInteraccionable interAcc { get; set; }

        public bool miraPalabras(string comando);
        public void ejecutar(); //QUE LE CORTEN LA CABEZA 
    }
}