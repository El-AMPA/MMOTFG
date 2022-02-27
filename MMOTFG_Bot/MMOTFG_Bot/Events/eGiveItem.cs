using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Events
{
    struct ItemLot
    {
        public string item
        {
            get;
            set;
        }
        public int quantity
        {
            get;
            set;
        }
        public float chanceToObtain
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

        public override void Execute(long chatId)
        {
            Console.WriteLine("TUPUTAMADRE\nTUPUTAMADRE\nTUPUTAMADRE\nTUPUTAMADRE\nTUPUTAMADRE\nTUPUTAMADRE\nTUPUTAMADRE\nTUPUTAMADRE\nTUPUTAMADRE\nTUPUTAMADRE\nTUPUTAMADRE\nTUPUTAMADRE\nTUPUTAMADRE\nTUPUTAMADRE\nTUPUTAMADRE\nTUPUTAMADRE\nTUPUTAMADRE\nTUPUTAMADRE\n");
            foreach(ItemLot i in ItemLots)
            {
                if(RNG.Next(0, 100) > i.chanceToObtain * 100){
                    InventorySystem.AddItem(chatId, i.item, i.quantity);
                }
            }
        }
    }
}
