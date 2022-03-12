using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class Tatirana : EquipableItem
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
            await TelegramCommunicator.SendText(chatId, "Eso no sería muy vegano, la verdad.");
        }

        public override async void OnEquip(long chatId, string[] args = null)
        {
            await TelegramCommunicator.SendText(chatId, "Te equipas con la Tatirana. Sientes que formáis un gran equipo y que podríais pasar el resto de vuestra vida juntos. Tu ataque y MP suben 15 puntos.");
            //BattleSystem.player.stats[(int)StatName.ATK] += 10;
        }

        public Tatirana()
        {
            name = "tatirana";
        }
    }
}
