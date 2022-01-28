namespace MMOTFG_Bot
{
    interface IInteraccionable
    {
        void sendToUser(int userID, string args);
        void moveTowards(int userID, Direction dir);
    }
}