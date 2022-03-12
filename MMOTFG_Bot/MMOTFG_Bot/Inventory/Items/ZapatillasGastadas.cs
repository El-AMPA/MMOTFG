using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class ZapatillasGastadas : EquipableItem
    {
        public override void Init()
        {
            base.Init();
            iD = Guid.NewGuid();
            gearSlot = EQUIPMENT_SLOT.FEET;
            key_words.Add(new KeyValuePair<string, Action<long, string[]>>("/equip", Eat));
        }

        private async void Eat(long chatId, string[] args)
        {
            await TelegramCommunicator.SendText(chatId, "Oye pues no están malas, empiezo a entender mejor a Gauss...");
        }

        public override async void OnEquip(long chatId, string[] args = null)
        {
            await TelegramCommunicator.SendText(chatId, "Te equipaste las zapatillas gastadas. Es casi mágico que no se hayan caído a trozos con lo maltratadas que están. Tu MP sube 10 puntos.");
            //BattleSystem.player.stats[(int)StatName.ATK] += 10;
        }

        public ZapatillasGastadas()
        {
            name = "zapatillas_gastadas";
        }
    }
}
