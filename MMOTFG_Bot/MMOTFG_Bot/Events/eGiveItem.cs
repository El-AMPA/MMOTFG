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
        //Range [0, 1]
        public float ChanceToObtain
        {
            get;
            set;
        }
    }
    /// <summary>
    /// Grants the user items. Supports multiple items and random drop chance.
    /// </summary>
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
                if(RNG.Next(0, 100) > 100 - i.ChanceToObtain * 100){
                    await InventorySystem.AddItem(chatId, i.Item, i.Quantity);
                }
            }
        }
    }
}
