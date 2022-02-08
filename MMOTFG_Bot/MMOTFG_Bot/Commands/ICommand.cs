namespace MMOTFG_Bot
{
    abstract class ICommand
    {
        protected string[] key_words;

        public abstract void Init();

        //Checks if the string command exists in the key_words of the specific command
        public bool ContainsKeyWord(string command, long chatId, string[] args = null)
        {
            foreach (string p in key_words)
            {
                if (command == p)
                {
                    if (!IsFormattedCorrectly(args)) return false;
                    Execute(command, chatId, args);
                    return true;
                }
            }
            return false;
        }
        internal abstract void Execute(string command, long chatId, string[] args = null);

        //Determines wether the given order is formatted correctly or not.
        //(/add 10a apples isn't formatted correctly. This method should read this text and determine that it's not correct)
        internal abstract bool IsFormattedCorrectly(string[] args);
    }
}