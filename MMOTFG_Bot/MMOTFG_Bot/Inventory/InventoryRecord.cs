using System;
using System.Collections.Generic;
using System.Text;

namespace MMOTFG_Bot.Inventory
{
    class InventoryRecord
    {
        public ObtainableItem InventoryItem { get; private set; }
        public int Quantity { get; private set; }
        public InventoryRecord(ObtainableItem item, int quantity)
        {
            InventoryItem = item;
            Quantity = quantity;
        }
        public void AddToQuantity(int amountToAdd)
        {
            Quantity += amountToAdd;
        }

        public Dictionary<string, object> GetSerializable()
        {
            return new Dictionary<string, object>
                {
                    {InventoryItem.name, Quantity}
                };
        }

    }
}
