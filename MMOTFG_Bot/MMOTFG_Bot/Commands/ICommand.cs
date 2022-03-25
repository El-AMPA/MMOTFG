using System.Threading.Tasks;

namespace MMOTFG_Bot
{
    abstract class ICommand
    {
        protected string[] key_words;

        protected string commandDescription;

        /// <summary>
        /// Sets the keywords that will be recognized by this command
        /// </summary>
        public abstract void SetKeywords();

        /// <summary>
        /// Checks if the 'command' string exists in the registred key_words of this command
        /// </summary>
        public bool ContainsKeyWord(string command)
        {
            foreach (string p in key_words)
            {
                if (command == p)
                {
                    return true;
                }
            }
            return false;
        }

        public bool TryExecute(string command, long chatId, string[] args = null)
		{
            if (!IsFormattedCorrectly(args)) return false;
            Execute(command, chatId, args);
            return true;
        }

        /// <summary>
        /// Applies the effect of this command
        /// </summary>
        internal abstract void Execute(string command, long chatId, string[] args = null);


        /// <summary>
        /// Determines wether the given order is formatted correctly or not.
        /// /add 10a apples isn't formatted correctly. This method should read this text and determine that it's not correct)
        /// </summary>
        internal abstract bool IsFormattedCorrectly(string[] args);

        internal string[] getKeywords() {
            return key_words;
        }

        public string getDescription()
		{
            return commandDescription;
		}

        public abstract void setDescription();

    }
}