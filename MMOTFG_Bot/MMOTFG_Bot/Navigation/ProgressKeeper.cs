using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Navigation
{
    class ProgressKeeper
    {
        public static Dictionary<string, bool> PlayerRecords = new Dictionary<string, bool>();

        public static async Task CreateProgressKeeper(long chatId)
        {
            await SaveSerializable(chatId);
        }

        public static async Task LoadSerializable(long chatId)
        {
            PlayerRecords.Clear();

            //Loads the dictionary from the DB
            Dictionary<string, object> player = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID,
            chatId.ToString(), DbConstants.COLLEC_DEBUG);

            Dictionary<string, object> dbFlagDict = (Dictionary<string, object>)(player[DbConstants.PLAYER_FIELD_FLAGS]);

            foreach(KeyValuePair<string, object> flagInDict in dbFlagDict)
            {
                PlayerRecords.Add(flagInDict.Key, Convert.ToBoolean(flagInDict.Value));
            }
        }

        public static async Task SaveSerializable(long chatId)
        {
            //Loads the dictionary from the DB
            Dictionary<string, object> update = new Dictionary<string, object>()
            {
                {DbConstants.PLAYER_FIELD_FLAGS, PlayerRecords }
            };

            await DatabaseManager.ModifyDocumentFromCollection(update, chatId.ToString(), DbConstants.COLLEC_DEBUG);
        }

        public static void SetFlagAs(long chatId, string flag, bool active)
        {
            if (PlayerRecords.ContainsKey(flag)) PlayerRecords[flag] = active;
            else PlayerRecords.Add(flag, active);
        }

        public static bool IsFlagActive(long chatId, string flag)
        {
            string[] substrings = flag.Split(' ');

            if (substrings.Length == 1)
            {
                if (flag[0] == '!') return !PlayerRecords.ContainsKey(flag.Substring(1));
                else return PlayerRecords.ContainsKey(flag);
            }

            int operation = 0; //0 = override previous result. Useful at the beggining , 1 = AND, 2 = OR
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
                        bool current;
                        if (s[0] == '!') current = !PlayerRecords.ContainsKey(s.Substring(1));
                        else current = PlayerRecords.ContainsKey(s);

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
