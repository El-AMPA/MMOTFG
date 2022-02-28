using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    abstract class EquipableItem : ObtainableItem
    {
        public EQUIPMENT_SLOT gearSlot;

        public override void Init()
        {
            //key_words.Add(new KeyValuePair<string, Action<long, string[]>>("/equip", Equip));
        }

        private void OnEquip(long chatId, string[] args)
        {

        }

        private void OnUnequip(long chatId, string[] args)
        {

        }
    }
}
