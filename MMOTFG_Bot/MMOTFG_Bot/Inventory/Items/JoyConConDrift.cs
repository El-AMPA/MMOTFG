using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Items
{
    class JoyConConDrift : EquipableItem
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
            await TelegramCommunicator.SendText(chatId, "Le daré solo un mordisco, que no quiero emocionarme y que pase lo de la última vez.");
        }

        public override async void OnEquip(long chatId, string[] args = null)
        {
            await TelegramCommunicator.SendText(chatId, "Te equipaste el joy-con con drift. De lo mal hecho que está le hará daño moral a tus enemigos. Tu ataque sube 10 puntos.");
            //BattleSystem.player.stats[(int)StatName.ATK] += 10;
        }

        public JoyConConDrift()
        {
            name = "joycon_con_drift";
        }
    }
}
