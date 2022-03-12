using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class CamisetaDeEvangelion : EquipableItem
    {
        public override void Init()
        {
            base.Init();
            iD = Guid.NewGuid();
            gearSlot = EQUIPMENT_SLOT.CHEST;
            key_words.Add(new KeyValuePair<string, Action<long, string[]>>("/equip", Eat));
        }

        private async void Eat(long chatId, string[] args)
        {
            await TelegramCommunicator.SendText(chatId, "Qué asco, mejor la lavo antes de comérmela.");
        }

        public override async void OnEquip(long chatId, string[] args = null)
        {
            await TelegramCommunicator.SendText(chatId, "Te equipaste la camiseta de evangelion. Da un poco de cringe así que los enemigos serán más reacios a tocarte. Tu defensa sube 10 puntos.");
            //BattleSystem.player.stats[(int)StatName.ATK] += 10;
        }

        public CamisetaDeEvangelion()
        {
            name = "camiseta_de_evangelion";
        }
    }
}
