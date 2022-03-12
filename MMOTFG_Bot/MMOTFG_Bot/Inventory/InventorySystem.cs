using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMOTFG_Bot.Items;

namespace MMOTFG_Bot
{
    static class InventorySystem
    {
        private const int MAX_SLOTS_INVENTORY = 10;
        private static List<InventoryRecord> InventoryRecords = new List<InventoryRecord>();
        private static Dictionary<string, ObtainableItem> obtainableItems = new Dictionary<string, ObtainableItem>();
        private static EquipableItem[] equipment;

        public static void Init()
        {
			//TO-DO: ESTO ES FEO DE COJONES ME ESTOY MURIENDO DE VERLO
			equipment = new EquipableItem[Enum.GetNames(typeof(EQUIPMENT_SLOT)).Length];

			HealthPotion hPotion = new HealthPotion();
			hPotion.Init();

			ManaPotion mPotion = new ManaPotion();
			mPotion.Init();

			ThunderfuryBleesedBladeOfTheWindseeker tFury = new ThunderfuryBleesedBladeOfTheWindseeker();
			tFury.Init();

            SulfurasHandOfRagnaros hRag = new SulfurasHandOfRagnaros();
            hRag.Init();

			obtainableItems.Add(hPotion.name, hPotion);
			obtainableItems.Add(mPotion.name, mPotion);
			obtainableItems.Add(tFury.name, tFury);            
            obtainableItems.Add(hRag.name, hRag);
        }
		
   //     public static async Task saveToDatabase(long chatId)
   //     {
   //         //TO-DO: ESTO ES FEO DE COJONES ME ESTOY MURIENDO DE VERLO
   //         equipment = new EquipableItem[Enum.GetNames(typeof(EQUIPMENT_SLOT)).Length];

   //         HealthPotion hPotion = new HealthPotion();
   //         hPotion.Init();

   //         ManaPotion mPotion = new ManaPotion();
   //         mPotion.Init();

   //         ThunderfuryBleesedBladeOfTheWindseeker tFury = new ThunderfuryBleesedBladeOfTheWindseeker();
   //         tFury.Init();

   //         obtainableItems.Add(hPotion.name, hPotion);
   //         obtainableItems.Add(mPotion.name, mPotion);
   //         obtainableItems.Add(tFury.name, tFury);

   //         Dictionary<string, object> player = await DatabaseManager.getDocumentByUniqueValue("telegramId", chatId.ToString(), "PlayersPrueba");

   //         string[] itemsNames = new string[obtainableItems.Count];

   //         int i = 0;

   //         foreach(KeyValuePair<string,ObtainableItem> it in obtainableItems)
			//{
   //             itemsNames[i] = it.Key;
   //             i++;
			//}

   //         player.Add("Inventory", itemsNames);

   //         await DatabaseManager.modifyDocumentFromCollection(player, chatId.ToString(), "PlayersPrueba");
   //     }


        //TO-DO: Repensar si es mejor dejarlo como está o que al sistema de inventario le llegue la clase Objeto ya directamente. Es bastante inflexible solo poder recibir un string y
        //traducirlo aquí
        public static bool StringToItem(string s, out ObtainableItem item)
        {
            return obtainableItems.TryGetValue(s, out item);
        }

        public static bool StringToEquipmentSlot(string s, out EQUIPMENT_SLOT slot)
        {
            return Enum.TryParse(s, true, out slot);
        }

        public static async Task AddItem(long chatId, string itemString, int quantityToAdd)
        {
            ObtainableItem item;
            if (StringToItem(itemString, out item))
            {
                int quantityToAddAux = quantityToAdd;
                while (quantityToAddAux > 0)
                {
                    // If an object of this item type already exists in the inventory, and has room to stack more items,
                    // then add as many as we can to that stack.
                    if (InventoryRecords.Exists(x => (x.InventoryItem.iD == item.iD) && (x.Quantity < item.maxStackQuantity)))
                    {
                        InventoryRecord inventoryRecord = InventoryRecords.First(x => (x.InventoryItem.iD == item.iD) && (x.Quantity < item.maxStackQuantity));

                        // Calculate how many more can be added to this stack
                        int maximumQuantityYouCanAddToThisStack = (item.maxStackQuantity - inventoryRecord.Quantity);

                        // Add to the stack (either the full quanity, or the amount that would make it reach the stack maximum)
                        int quantityToAddToStack = Math.Min(quantityToAddAux, maximumQuantityYouCanAddToThisStack);

                        inventoryRecord.AddToQuantity(quantityToAddToStack);

                        // Decrease the quantityToAdd by the amount we added to the stack.
                        // If we added the total quantityToAdd to the stack, then this value will be 0, and we'll exit the 'while' loop.
                        quantityToAddAux -= quantityToAddToStack;
                    }
                    else
                    {
                        // We don't already have an existing inventoryRecord for this ObtainableItem object,
                        // so, add one to the list, if there is room.
                        if (InventoryRecords.Count < MAX_SLOTS_INVENTORY)
                        {
                            // Don't set the quantity value here.
                            // The 'while' loop will take us back to the code above, which will add to the quantity.
                            InventoryRecords.Add(new InventoryRecord(item, 0));
                        }
                        else
                        {
                            //There is no available space in the inventory
                            await TelegramCommunicator.SendText(chatId, "Item " + item.name + " couldn't be added because inventory is full.");
                            break;
                        }
                    }
                }
                if (quantityToAdd == 1) await TelegramCommunicator.SendText(chatId, "Item " + item.name + " was added to the inventory.");
                else await TelegramCommunicator.SendText(chatId, "Item " + item.name + " was added " + (quantityToAdd - quantityToAddAux) + " times");
            }
            else await TelegramCommunicator.SendText(chatId, "Item " + itemString + " doesn't exist");
        }

        public static async Task ConsumeItem(long chatId, string itemString, int quantityToConsume, string command = null, string[] args = null)
        {
            ObtainableItem item;
            if (StringToItem(itemString, out item))
            {
                if (command != null && !item.UnderstandsCommand(command))
                {
                    await TelegramCommunicator.SendText(chatId, "Can't do that with that item");
                    return;
                }

                //If the item isn't contained in the inventory, there is no point in continuing.
                if (!InventoryRecords.Exists(x => x.InventoryItem.iD == item.iD))
                {
                    await TelegramCommunicator.SendText(chatId, "Item " + item.name + " couldn't be consumed as it was not found in your inventory");
                    return;
                }

                int quantityToConsumeAux = quantityToConsume;
                if (quantityToConsumeAux == -1)
                {
                    quantityToConsume = GetNumberOfItemsInInventory(chatId, item); //-1 = Every single item of that type
                    quantityToConsumeAux = quantityToConsume;
                }
                while (quantityToConsumeAux > 0 && InventoryRecords.Exists(x => (x.InventoryItem.iD == item.iD)))
                {
                    // If an object of this item type already exists in the inventory, and has room to stack more items,
                    // then add as many as we can to that stack.
                    InventoryRecord inventoryRecord = InventoryRecords.First(x => (x.InventoryItem.iD == item.iD));

                    // Add to the stack (either the full quanity, or the amount that would make it reach the stack maximum)
                    int quantityToConsumeToStack = Math.Min(quantityToConsumeAux, inventoryRecord.Quantity);

                    if(command != null)
                    {
                        for (int k = 0; k < quantityToConsumeToStack; k++)
                        {
                            item.ProcessCommand(command, chatId, args);
                        }
                    }
                    inventoryRecord.AddToQuantity(-quantityToConsumeToStack);

                    //If the current stack has been deplenished, it's removed from the list
                    if (inventoryRecord.Quantity == 0) InventoryRecords.Remove(inventoryRecord);

                    // Decrease the quantityToConsume by the amount we added to the stack.
                    // If we added the total quantityToConsume to the stack, then this value will be 0, and we'll exit the 'while' loop.
                    quantityToConsumeAux -= quantityToConsumeToStack;
                }
                if (quantityToConsumeAux > 0)
                {
                    //Couldn't consume every item.
                }
                if (quantityToConsume == 1) await TelegramCommunicator.SendText(chatId, "Item " + item.name + " was consumed.");
                else await TelegramCommunicator.SendText(chatId, "Item " + item.name + " was consumed " + (quantityToConsume - quantityToConsumeAux) + " times");
            }
            else await TelegramCommunicator.SendText(chatId, "Item " + itemString + " doesn't exist");
        }

        public static async Task ThrowAwayItem(long chatId, string itemString, int quantityToThrowAway)
        {
            ObtainableItem item;
            if (StringToItem(itemString, out item))
            {
                //If the item isn't contained in the inventory, there is no point in continuing.
                if (!InventoryRecords.Exists(x => x.InventoryItem.iD == item.iD))
                {
                    await TelegramCommunicator.SendText(chatId, "Item " + item.name + " couldn't be thrown away as it was not found in your inventory");
                    return;
                }

                int quantityToThrowAwayAux = quantityToThrowAway;
                if (quantityToThrowAwayAux == -1)
                {
                    quantityToThrowAway = GetNumberOfItemsInInventory(chatId, item); //-1 = Every single item of that type
                    quantityToThrowAwayAux = quantityToThrowAway;
                }

                while (quantityToThrowAwayAux > 0 && InventoryRecords.Exists(x => (x.InventoryItem.iD == item.iD)))
                {
                    // If an object of this item type already exists in the inventory, and has room to stack more items,
                    // then add as many as we can to that stack.
                    InventoryRecord inventoryRecord = InventoryRecords.First(x => (x.InventoryItem.iD == item.iD));

                    // Add to the stack (either the full quanity, or the amount that would make it reach the stack maximum)
                    int quantityToAddToStack = Math.Min(quantityToThrowAwayAux, inventoryRecord.Quantity);

                    inventoryRecord.AddToQuantity(-quantityToAddToStack);

                    //If the current stack has been deplenished, it's removed from the list
                    if (inventoryRecord.Quantity == 0) InventoryRecords.Remove(inventoryRecord);

                    // Decrease the quantityToThrowAway by the amount we added to the stack.
                    // If we added the total quantityToThrowAway to the stack, then this value will be 0, and we'll exit the 'while' loop.
                    quantityToThrowAwayAux -= quantityToAddToStack;
                }
                if (quantityToThrowAwayAux > 0)
                {
                    //Couldn't consume every item.
                }
                if (quantityToThrowAway == 1) await TelegramCommunicator.SendText(chatId, "Item " + item.name + " was thrown away.");
                else await TelegramCommunicator.SendText(chatId, "Item " + item.name + " was thrown away " + (quantityToThrowAway - quantityToThrowAwayAux) + " times");
            }
            else await TelegramCommunicator.SendText(chatId, "Item " + itemString + " doesn't exist");
        }

        public static int GetNumberOfItemsInInventory(long chatId, ObtainableItem item)
        {
            int numItems = 0;
            List<InventoryRecord> auxRecord = InventoryRecords.FindAll(x => x.InventoryItem.iD == item.iD); //TO-DO: Cuando se haga un refactor de los items, comprar por ID's, no por nombres
            foreach (InventoryRecord i in auxRecord)
            {
                numItems += i.Quantity;
            }

            return numItems;
        }

        public static async Task ShowInventory(long chatId)
        {
            string message = "User inventory:\n";
            foreach (InventoryRecord i in InventoryRecords)
            {
                message += i.InventoryItem.name + " x" + i.Quantity + "\n";
            }

            if (message != "") await TelegramCommunicator.SendText(chatId, message);
        }

        /// <summary>
        /// Shows the player's equipped items from all gear slots
        /// </summary>
        public static async Task ShowGear(long chatId)
        {
            string message = "User equipment:\n";
            for(int k = 0; k < equipment.Length; k++)
            {
                message += "\n"+(EQUIPMENT_SLOT)k + ": ";
                if (equipment[k] == null) message += " empty";
                else message += equipment[k].name;
            }

            if (message != "") await TelegramCommunicator.SendText(chatId, message);
        }

        /// <summary>
        /// Shows the player's equipped items from the specified gear slot
        /// </summary>
        public static async Task ShowGear(long chatId, EQUIPMENT_SLOT slot)
        {
            string message = "User equipment on " + slot + " slot: ";
            if (equipment[(int)slot] == null) message += " empty";
            else message += equipment[(int)slot].name;

            if (message != "") await TelegramCommunicator.SendText(chatId, message);
        }

        /// <summary>
        /// Unequips the item worn on the specified gear slot
        /// </summary>
        public static async Task UnequipGear(long chatId, EQUIPMENT_SLOT slot)
        {
            if (equipment[(int)slot] != null)
            {
                EquipableItem item = equipment[(int)slot];
                item.OnUnequip(chatId);

                string msg = "You have unequipped " + item.name + " from your " + item.gearSlot.ToString().ToLower() + " slot.";
                if (item.statModifiers.Count > 0)
                {
                    foreach (var stat in item.statModifiers)
                    {
                        msg += "\n" + stat.Item2 + " ";
                        if (stat.Item1 >= 0) msg += "-" + stat.Item1;
                        else msg += "+" + Math.Abs(stat.Item1);
                    }
                }
                await TelegramCommunicator.SendText(chatId, msg);

                equipment[(int)slot] = null;

                //Remove the item from the inventory
                await AddItem(chatId, item.name, 1);
            }
            else
            {
                await TelegramCommunicator.SendText(chatId, "Couldn't unequip an item from your " + slot.ToString().ToLower() + " gear slot because it's empty.");
            }
        }

        /// <summary>
        /// Equips a piece of gear in its gear slot
        /// </summary>
        public static async Task EquipGear(long chatId, EquipableItem item)
        {
            if(!InventoryRecords.Exists(x => x.InventoryItem.iD == item.iD))
            {
                await TelegramCommunicator.SendText(chatId, "Item " + item.name + " couldn't be equipped as it was not found in your inventory");
            }
            else
            {
                if (equipment[(int)item.gearSlot] != null)
                {
                    if (item.iD == equipment[(int)item.gearSlot].iD) await TelegramCommunicator.SendText(chatId, "You are already using that item");
                    else await SwapGear(chatId, item);
                }
                else
                {
                    item.OnEquip(chatId);

                    string msg = "You have equipped " + item.name + "on your " + item.gearSlot.ToString().ToLower() + " slot.";
                    if (item.statModifiers.Count > 0)
                    {
                        foreach (var stat in item.statModifiers)
                        {
                            msg += "\n" + stat.Item2 + " ";
                            if (stat.Item1 >= 0) msg += "+" + stat.Item1;
                            else msg += stat.Item1;
                        }
                    }
                    await TelegramCommunicator.SendText(chatId, msg);

                    equipment[(int)item.gearSlot] = item;

                    //Remove the item from the inventory
                    await ConsumeItem(chatId, item.name, 1);
                }
            }
        }

        /// <summary>
        /// Swaps the current equipped piece of gear for a new one. Shows the change of stats.
        /// </summary>
        private static async Task SwapGear(long chatId, EquipableItem newItem)
        {
            EquipableItem oldItem = equipment[(int)newItem.gearSlot];
            string msg = "You've swapped " + oldItem.name + " for " + newItem.name;
            List<(int, StatName)> auxChanges = new List<(int, StatName)>();

            for (int k = 0; k < oldItem.statModifiers.Count; k++)
            {
                auxChanges.Add((-oldItem.statModifiers[k].Item1, oldItem.statModifiers[k].Item2));
            }

            foreach (var statsNewItem in newItem.statModifiers)
            {
                bool found = false;
                for(int k = 0; k < oldItem.statModifiers.Count && !found; k++)
                {
                    //If both items change the same stat
                    if(statsNewItem.Item2 == oldItem.statModifiers[k].Item2)
                    {
                        auxChanges[k] = (statsNewItem.Item1 - oldItem.statModifiers[k].Item1, oldItem.statModifiers[k].Item2);
                        found = true;
                    }
                }
                //NewItem modifies stats that OldItem doesn't modify
                if (!found) auxChanges.Add(statsNewItem);
            }

            foreach(var stat in auxChanges)
            {
                msg += "\n" + stat.Item2 + " ";
                if (stat.Item1 >= 0) msg += "+" + stat.Item1;
                else msg += stat.Item1;
            }
            await TelegramCommunicator.SendText(chatId, msg);

            //Add the unequipped item to the inventory
            await AddItem(chatId, oldItem.name, 1);
            //Remove the item from the inventory
            await ConsumeItem(chatId, newItem.name, 1);

            equipment[(int)newItem.gearSlot].OnUnequip(chatId);
            equipment[(int)newItem.gearSlot] = newItem;
            equipment[(int)newItem.gearSlot].OnEquip(chatId);
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
