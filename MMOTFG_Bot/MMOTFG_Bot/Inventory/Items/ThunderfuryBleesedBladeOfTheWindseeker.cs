using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class ThunderfuryBleesedBladeOfTheWindseeker : EquipableItem
    {
        public override void Init()
        {
            base.Init();
            iD = Guid.NewGuid();
            gearSlot = EQUIPMENT_SLOT.WEAPON;
            key_words.Add(new KeyValuePair<string, Action<long, string[]>>("/equip", Eat));
        }

        private async void Eat(long chatId, string[] args)
        {
            await TelegramCommunicator.SendText(chatId, "Om nom nom");
        }

        public ThunderfuryBleesedBladeOfTheWindseeker()
        {
            name = "thunderfury_blessed_blade_of_the_windseeker";
        }
    }
}
