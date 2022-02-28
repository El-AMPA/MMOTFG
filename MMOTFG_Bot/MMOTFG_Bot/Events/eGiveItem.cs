using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Events
{
    struct ItemLot
    {
        public string Item
        {
            get;
            set;
        }
        public int Quantity
        {
            get;
            set;
        }
        public float ChanceToObtain
        {
            get;
            set;
        }
    }
    class eGiveItem : Event
    {
        public List<ItemLot> ItemLots
        {
            get;
            set;
        }

        public async override void Execute(long chatId)
        {
            foreach(ItemLot i in ItemLots)
            {
                if(RNG.Next(0, 100) > i.ChanceToObtain * 100){
                    await InventorySystem.AddItem(chatId, i.Item, i.Quantity);
                }
            }
        }
    }
}
