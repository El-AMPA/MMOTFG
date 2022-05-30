using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MMOTFG_Bot.Persistency;

namespace MMOTFG_Bot.Navigation
{
    class ProgressKeeper
    {
        public static Dictionary<string, bool> PlayerRecords = new Dictionary<string, bool>();

        public static async Task CreateProgressKeeper(string chatId)
        {
            PlayerRecords.Clear();
            await SaveSerializable(chatId);
        }

        public static async Task LoadSerializable(string chatId)
        {
            PlayerRecords.Clear();

            //Loads the dictionary from the DB
            Dictionary<string, object> player = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID,
            chatId, DbConstants.COLLEC_PLAYERS);

            Dictionary<string, object> dbFlagDict = (Dictionary<string, object>)(player[DbConstants.PLAYER_FIELD_FLAGS]);

            //Dumps the information from the DB into a dictionary
            foreach(KeyValuePair<string, object> flagInDict in dbFlagDict)
            {
                PlayerRecords.Add(flagInDict.Key, Convert.ToBoolean(flagInDict.Value));
            }
        }

        public static async Task SaveSerializable(string chatId)
        {
            //Dumps the information from the dictionary to a different dictionary that the DB understands
            Dictionary<string, object> update = new Dictionary<string, object>()
            {
                {DbConstants.PLAYER_FIELD_FLAGS, PlayerRecords }
            };

            //Uploads it into the DB
            await DatabaseManager.ModifyDocumentFromCollection(update, chatId, DbConstants.COLLEC_PLAYERS);
        }

        /// <summary>
        /// Sets a flag to a given value. If the flag doesn't exist in the DB, it creates a new entry.
        /// </summary>
        public static void SetFlagAs(string chatId, string flag, bool active)
        {
            if (PlayerRecords.ContainsKey(flag)) PlayerRecords[flag] = active;
            else PlayerRecords.Add(flag, active);
        }

        /// <summary>
        /// Returns wether or not a given flag is active. If the flag doesn't exist, it returns false.
        /// </summary>
        public static bool IsFlagActive(string chatId, string flag)
        {
            //Supports complex operations (x OR y), so it needs to be split into parts
            string[] substrings = flag.Split(' ');

            if (substrings.Length == 1)
            {
                if (flag[0] == '!') {
                    if (!PlayerRecords.TryGetValue(flag.Substring(1), out bool val)) return true;
                    else return !val;
                }
                else
                {
                    if (!PlayerRecords.TryGetValue(flag, out bool val)) return false;
                    else return val;
                }
            }

            int operation = 0; //0 = override previous result, 1 = AND, 2 = OR
            //Final result
            bool result = true;

            foreach (string s in substrings)
            {
                switch (s)
                {
                    case ("OR"):
                    case ("or"):
                        operation = 1;
                        break;
                    case ("AND"):
                    case ("and"):
                        operation = 2;
                        break;
                    default:
                        //If it's a flag...
                        bool current;
                        //If it doesn't exist, return false, or true if '!' was added at the beggining of the flag
                        if (s[0] == '!')
                        {
                            if (!PlayerRecords.TryGetValue(s.Substring(1), out current)) current = true;
                            else current = !current;
                        }
                        else if (!PlayerRecords.TryGetValue(s, out current)) current = false;

                        switch (operation)
                        {
                            case (0):
                                result = current;
                                break;
                            case (1):
                                result = result || current;
                                break;
                            case (2):
                                result = result && current;
                                break;
                        }
                        break;
                }
            }
            return result;
        }
    }
}
