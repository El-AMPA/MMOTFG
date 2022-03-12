using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class CollarDeJorge : EquipableItem
    {
        public override void Init()
        {
            base.Init();
            iD = Guid.NewGuid();
            gearSlot = EQUIPMENT_SLOT.TRINKET;
            key_words.Add(new KeyValuePair<string, Action<long, string[]>>("/equip", Eat));
        }

        private async void Eat(long chatId, string[] args)
        {
            await TelegramCommunicator.SendText(chatId, "Si quiero comer hierro mejor me hago unas lentejas.");
        }

        public override async void OnEquip(long chatId, string[] args = null)
        {
            await TelegramCommunicator.SendText(chatId, "Te equipaste el collar de Jorge. Con solo acercarte a un enemigo ya le haces daño visual. Tu ataque sube 20 puntos.");
            //BattleSystem.player.stats[(int)StatName.ATK] += 10;
        }

        public CollarDeJorge()
        {
            name = "collar_de_jorge";
        }
    }
}
