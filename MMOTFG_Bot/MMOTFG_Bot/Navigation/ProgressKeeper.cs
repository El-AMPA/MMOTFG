using System;
using System.Collections.Generic;
using System.Text;
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

        public static bool IsFlagActive(long chatId, string flag)
        {
            //Checks if the flag is active in the DB
            return PlayerRecords.ContainsKey(flag);
        }

        public static void SetFlagAs(long chatId, string flag, bool active)
        {
            if (PlayerRecords.ContainsKey(flag)) PlayerRecords[flag] = active;
            else PlayerRecords.Add(flag, active);
        }
    }
}
