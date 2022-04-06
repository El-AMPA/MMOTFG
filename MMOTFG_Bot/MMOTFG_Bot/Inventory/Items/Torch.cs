using System;
using System.Collections.Generic;
using System.Text;
using MMOTFG_Bot.Navigation;

namespace MMOTFG_Bot.Items
{
    class Torch : EquipableItem
    {
        public override void Init()
        {
            gearSlot = EQUIPMENT_SLOT.CHEST;
            statModifiers = new List<(int, StatName)>
            {
                (-2, StatName.ATK),
                (10, StatName.MP)
            };
        }

        public override async void OnEquip(string chatId, string[] args = null)
        {
            base.OnEquip(chatId, args);

            await ProgressKeeper.LoadSerializable(chatId);
            ProgressKeeper.SetFlagAs(chatId, "TorchLit", true);
            await ProgressKeeper.SaveSerializable(chatId);
        }

        public override async void OnUnequip(string chatId, string[] args = null)
        {
            base.OnUnequip(chatId, args);

            await ProgressKeeper.LoadSerializable(chatId);
            ProgressKeeper.SetFlagAs(chatId, "TorchLit", false);
            await ProgressKeeper.SaveSerializable(chatId);
        }

        public Torch()
        {
            name = "torch";
        }
    }
}
