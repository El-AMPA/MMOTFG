using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MMOTFG_Bot.Battle.Enemies
{
    class Tinkyexe : Enemy
    {
        public Tinkyexe()
        {
            name = "Tinky.exe";

            imageName = "tinkyexe.png";
            imageCaption = "T̵i̶n̵k̴y̸";
            droppedMoney = 0;
            droppedItem = new Items.HealthPotion();
            droppedItemAmount = 2;

            stats = new float[]{100, 100, 100};
            originalStats = (float[])stats.Clone();

            attacks = new Attack[]{
                new Attack("t̶͖̮̙̦̯̮͓̲̎u̴̧̡̢͓̮͉͓̻͙̰̟̳͖̠̇͆̽̀͂̈̈̆ ̷̡̛̭̩̲̦͉̱͓̈́̄͑c̷͕̜͎͚̑́̂͑́ỏ̶̧̢̩̳̗̤̎̄͠d̸̮̮͍͚̜̝̋̃̔̽̃̐̎̏̕͘͜͝ͅì̴̥̈́̕̚g̸̡͇̖̞̯̯̑ơ̸̡̙̳̝̟̐̒̾̈́͒̔̕ ̷̧̡̧̫̤̮̳̦͈̩̫̪̬̐́͆̈̿͜ȩ̵̡̨̨̪͇͇̼͎̳͓̣̥̙̺̃s̵̟̘͈̖̘͔̣͛̔̓͒ ̸̬̑̈̾s̴̢̺͇͎̈́̂̇̓̄̎͋e̴̡̺̙͉̥̳͕̣̤̩͔͉̽̀͆̔̂͐̆̓̔̈́̚͜͝ͅň̶̞͑̍́͊̑̃̅̎̏͜͝c̶̨̥͈̙͛̄͛̓̾̕̕ͅi̶̳͈̼͈̰̲͖͗̅l̶̥̞̽͛̾̎̂͐̓̾̆̀̓̓̐̕͝l̸̫̤̥̹̫̅̀͗̐́̽̃̆̆̉̀́͊͘͝ȧ̵̮̱͇̱͉̯̱̲̝̣̦̲͋̑͛̈́̿̈́͊́̑̈́̒̏̚͜͜m̶̹̩̖̠̀͌̑̄͗̉ę̶̧̦̙̰̤̫̤̘͇̫̠̘͊̈́̓͊͐̊̄̚͝͝͝ͅn̵̨̙̭̣͉̤̱̮̙̤̣͎͑̂́́͒͑͗t̵̡͚̣̖͊̇̄̈́̓́͌̔̏͝e̷͙̜̳̠͍̠̦̓̈̄̾̍ͅ ̵͖̘̭̝͈̯̩̪̩͈̳͖͆͑̽̓͂͋̈͑̎͘̕͜͜ḻ̸̗͇͇͇͕̹̬̝͚̺̐̈̌̀̈̌͊̏͌̍̀̓͘͘å̵̧̛̗̞͉̮̽͌̎̆̎͗̇͌̾̂̾͝m̷͉͕̞̳͍͙̣͌̈̈̏̾͛̍͋̄͂͛̚e̸̡͈̹̩̯̭̝͗̅̀́͗̑̑͝n̵̛̯͖͈̩̩͚͖̥͍̬͖̹̮͑̐̿̉͌͆̐̐̓́͜͜͠t̵̛͖̥͍̎̋͌͊̀̿̕̕a̴̛̗̐͌͑̊͌͋͑̈́̆͂͋̈́̈́͘ḅ̴̧͕͙̗͙̩̖̦͓̓̆͘ļ̸̢͕̞̦͚̰͚̝̲̲̃ề̷̛̗̠̟͊͌͐̂̃̀͘͝", 100, 0),
            };

            attackNum = attacks.Length;
        }

        public override async Task OnHit(long chatId)
        {
            await TelegramCommunicator.SendText(chatId, "n̵͔͚̟̈o̸̳̐̾͘ ̵̢̠̋͑p̶̰̦͂u̸̮͒e̷͈̰̜͗́d̵̦̻̦̔̾e̶̼̍̀̚s̵̢̮͉̑̈́̎ ̵͔̣̌̎͑p̵̛̞͇̎ḁ̷̛̉̂ş̸͓̺͠ȧ̸̦͙̥͝r̶̨͐t̸͔̻̋ë̵́͜ ̷̡̭̤̽e̶̙͒š̷̛̲̬t̴̡͖̕e̷̩̳̦͋̾ ̷̻͔̽j̷͎̇͊ę̷̗̋ḟ̶̫̦e̷͉͚̥͊̎̊ ̵̘͍͊̅͐n̶̻̓͝o̵̘̓̒̃ ̵̨͔̲̊͊̐h̵̬͆à̷̝͝y̷̢̆͐ ̸̺͆̉m̶̟͉͆͋͝á̵͕s̴̭̪̙̀̈́͛ ̷̳͚̒c̶̩̦̾͊o̵̟̭̍̊n̷̝̞̬̋̒ţ̴̜͊ȅ̴͚n̵̛͐̈́͜ĭ̷̞̈́͘d̵̯̯̳͝o̵̘͔̍̉̀ ̸͖̭̘̀̅͝a̷͕̽͒ ̶̞̼͕̎̓͝p̷͚̚ͅa̸͉̓͊͊r̵̭͕͐̿͝t̷͎̔̽i̴͕͌̈́r̵̻̻̅̒ ̷̬̲̳̃d̷͙̲͗ͅě̵̩̫̈ ̷̝̓͗͘a̴̰̫͑͜q̷̘͙͈̄̆ṵ̴̧̔́ͅi̵̧̤̠̋̌");
        }
    }
}
