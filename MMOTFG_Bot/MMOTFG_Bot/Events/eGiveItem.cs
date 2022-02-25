using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Events
{
    struct ItemLot
    {
        public ObtainableItem item;
        public int quantity;
        public float chanceToObtain;
    }
    class eGiveItem : Event
    {
        public List<ItemLot> ItemLots
        {
            get;
            set;
        }

        public override void Execute(long chatId)
        {
            foreach(ItemLot i in ItemLots)
            {
                if(RNG.Next(0, 100) > i.chanceToObtain * 100){
                    InventorySystem.AddItem(chatId, i.item.name, i.quantity);
                }
            }
        }
    }
}
