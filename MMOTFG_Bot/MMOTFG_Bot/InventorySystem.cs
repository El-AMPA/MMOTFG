using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMOTFG_Bot
{
    class InventorySystem
    {
        private const int MAX_SLOTS_INVENTORY = 10;
        public readonly List<InventoryRecord> InventoryRecords = new List<InventoryRecord>();
        public void AddItem(ObtainableItem item, int quantityToAdd)
        {
            while (quantityToAdd > 0)
            {
                // If an object of this item type already exists in the inventory, and has room to stack more items,
                // then add as many as we can to that stack.
                if (InventoryRecords.Exists(x => (x.InventoryItem.iD == item.iD) && (x.Quantity < item.maxStackQuantity)))
                {
                    InventoryRecord inventoryRecord = InventoryRecords.First(x => (x.InventoryItem.iD == item.iD) && (x.Quantity < item.maxStackQuantity));

                    // Calculate how many more can be added to this stack
                    int maximumQuantityYouCanAddToThisStack = (item.maxStackQuantity - inventoryRecord.Quantity);

                    // Add to the stack (either the full quanity, or the amount that would make it reach the stack maximum)
                    int quantityToAddToStack = Math.Min(quantityToAdd, maximumQuantityYouCanAddToThisStack);

                    inventoryRecord.AddToQuantity(quantityToAddToStack);

                    // Decrease the quantityToAdd by the amount we added to the stack.
                    // If we added the total quantityToAdd to the stack, then this value will be 0, and we'll exit the 'while' loop.
                    quantityToAdd -= quantityToAddToStack;
                }
                else
                {
                    //O el objeto no existe o has llegado al cupo 
                }
            }
        }

        public class InventoryRecord
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

        }
    }
}
