using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class VaquerosNormales : EquipableItem
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
            await TelegramCommunicator.SendText(chatId, "Uf, paso, soy más de zapatillas.");
        }

        public override async void OnEquip(long chatId, string[] args = null)
        {
            await TelegramCommunicator.SendText(chatId, "Te equipaste los pantalones normales. Por fin la gente dejará de mirarte raro por ir en calzoncillos. Tu defensa sube 10 puntos.");
            //BattleSystem.player.stats[(int)StatName.ATK] += 10;
        }

        public VaquerosNormales()
        {
            name = "vaqueros_normales";
        }
    }
}
