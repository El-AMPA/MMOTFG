namespace MMOTFG_Bot
{
    abstract class ICommand
    {
        private string[] key_words;

        public abstract bool ContainsKeyWord(string command, long chatId, string[] args = null);
        internal abstract void Execute(string command, long chatId, string[] args = null); //QUE LE CORTEN LA CABEZA

        internal abstract bool IsFormattedCorrectly(string[] args);
    }
}