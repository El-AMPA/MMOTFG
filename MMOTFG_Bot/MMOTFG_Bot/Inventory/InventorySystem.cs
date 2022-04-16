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
        private static EquipableItem[] equipment;
        private static string currentChatid = "";

        public static void Init()
        {
            //TO-DO: ESTO ES FEO DE COJONES ME ESTOY MURIENDO DE VERLO
            equipment = new EquipableItem[Enum.GetNames(typeof(EQUIPMENT_SLOT)).Length];
        }

        /// <summary>
        /// Resets the inventory records and the equipment for a different player
        /// </summary>
        private static void Reset()
        {
            InventoryRecords = new List<InventoryRecord>();
            equipment = new EquipableItem[Enum.GetNames(typeof(EQUIPMENT_SLOT)).Length];
            currentChatid = "";
        }

        /// <summary>
        /// Loads to memory the inventory of the player with the given chatId
        /// </summary>
        public static async Task LoadPlayerInventory(string chatId)
        {
            if (currentChatid == chatId) return;

            Reset();

            Dictionary<string, object> player = await DatabaseManager.GetDocumentByUniqueValue(DbConstants.PLAYER_FIELD_TELEGRAM_ID,
                chatId.ToString(), DbConstants.COLLEC_DEBUG);


            //Inventario "normal" (no equipables)
            List<object> dbInventory = (List<object>)(player[DbConstants.PLAYER_FIELD_INVENTORY]);

            foreach (Dictionary<string, object> itemAmountDict in dbInventory)
            {
                foreach (KeyValuePair<string, object> itemAmountEntry in itemAmountDict)
                {
                    StringToItem(itemAmountEntry.Key, out ObtainableItem item);
                    InventoryRecords.Add(new InventoryRecord(item, Convert.ToInt32(itemAmountEntry.Value)));
                }
            }

            //Inventario de equipables
            Dictionary<string, object> dbEquipment = (Dictionary<string, object>)(player[DbConstants.PLAYER_FIELD_EQUIPABLE_ITEMS]);


            foreach (KeyValuePair<string, object> equiItem in dbEquipment)
            {
                ObtainableItem item = null;
                Enum.TryParse(typeof(EQUIPMENT_SLOT), equiItem.Key.ToString(), true, out object index);

                if (equiItem.Value != null) { 
                    StringToItem(equiItem.Value.ToString(), out item);               
                }

                equipment[Convert.ToInt32(index)] = (EquipableItem)item;
            }

            currentChatid = chatId;
        }

        /// <summary>
        /// Saves to the database the data (currently on memory) of the player with the given chatId
        /// </summary>
        public static async Task SavePlayerInventory(string chatId)
        {
            Dictionary<string, object> update = new Dictionary<string, object>();

            update.Remove(DbConstants.PLAYER_FIELD_INVENTORY);
            update.Remove(DbConstants.PLAYER_FIELD_EQUIPABLE_ITEMS);

            //preparamos los objetos stackeables normales
            Dictionary<string, object>[] invRecordsToSave = new Dictionary<string, object>[InventoryRecords.Count];

            int i = 0;
            foreach (InventoryRecord temp in InventoryRecords)
            {
                invRecordsToSave[i] = temp.GetSerializable();
                i++;
            }
            update.Add(DbConstants.PLAYER_FIELD_INVENTORY, invRecordsToSave);


            //preparamos los equipables
            Dictionary<string, object> equipItemsToSave = new Dictionary<string, object>();

            foreach (int equipNumbers in Enum.GetValues(typeof(EQUIPMENT_SLOT)))
            {
                object itemName = equipment[equipNumbers] is null ? null : equipment[equipNumbers].name;
                equipItemsToSave.Add(Enum.GetName(typeof(EQUIPMENT_SLOT), equipNumbers), itemName);
            }
            update.Add(DbConstants.PLAYER_FIELD_EQUIPABLE_ITEMS, equipItemsToSave);

            //actualizamos
            await DatabaseManager.ModifyDocumentFromCollection(update, chatId.ToString(), DbConstants.COLLEC_DEBUG);
        }

        /// <summary>
        /// Creates and saves to the database an empty inventory for the given player
        /// </summary>
        public static async Task CreatePlayerInventory(string chatId)
		{
            Reset();
            await SavePlayerInventory(chatId);
		}

        //TO-DO: Repensar si es mejor dejarlo como está o que al sistema de inventario le llegue la clase Objeto ya directamente. Es bastante inflexible solo poder recibir un string y
        //traducirlo aquí
        public static bool StringToItem(string s, out ObtainableItem item)
        {
            item = JSONSystem.GetItem(s);
            return item != null;
        }

        public static bool StringToEquipmentSlot(string s, out EQUIPMENT_SLOT slot)
        {
            return Enum.TryParse(s, true, out slot);
        }

        public static async Task AddItem(string chatId, string itemString, int quantityToAdd)
        {
            await LoadPlayerInventory(chatId);

            if (StringToItem(itemString, out ObtainableItem item))
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

            await SavePlayerInventory(chatId);
        }

        public static async Task<EquipableItem> GetItemFromEquipmentSlot(string chatId, EQUIPMENT_SLOT slot)
        {
            await LoadPlayerInventory(chatId);

            return equipment[(int)slot];
        }

        public static async Task ConsumeItem(string chatId, string itemString, int quantityToConsume, string command = null, string[] args = null)
        {
            await LoadPlayerInventory(chatId);
            if (StringToItem(itemString, out ObtainableItem item))
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
                    quantityToConsume = await GetNumberOfItemsInInventory(chatId, item); //-1 = Every single item of that type
                    quantityToConsumeAux = quantityToConsume;
                }
                //Check if the player in currently in a battle
                bool playerInBattle = await BattleSystem.IsPlayerInBattle(chatId);
                if (playerInBattle)
                { //If the user is on a battle, it can only use 1 item in it's turn.
                    if (quantityToConsume > 1)
                    {
                        await TelegramCommunicator.SendText(chatId, "You can only use 1 item on your turn.");
                        return;
                    }
                }
                while (quantityToConsumeAux > 0 && InventoryRecords.Exists(x => (x.InventoryItem.iD == item.iD)))
                {
                    // If an object of this item type already exists in the inventory, and has room to stack more items,
                    // then add as many as we can to that stack.
                    InventoryRecord inventoryRecord = InventoryRecords.First(x => (x.InventoryItem.iD == item.iD));

                    // Add to the stack (either the full quanity, or the amount that would make it reach the stack maximum)
                    int quantityToConsumeToStack = Math.Min(quantityToConsumeAux, inventoryRecord.Quantity);

                    if (command != null)
                    {
                        for (int k = 0; k < quantityToConsumeToStack; k++)
                        {
                            await item.ProcessCommand(command, chatId, args);
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
                //enemie's turn
                if (playerInBattle) await BattleSystem.player.SkipTurn(chatId);

                await SavePlayerInventory(chatId);
            }
            else await TelegramCommunicator.SendText(chatId, "Item " + itemString + " doesn't exist");
        }

        public static async Task ThrowAwayItem(string chatId, string itemString, int quantityToThrowAway)
        {
            await LoadPlayerInventory(chatId);

            if (StringToItem(itemString, out ObtainableItem item))
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
                    quantityToThrowAway = await GetNumberOfItemsInInventory(chatId, item); //-1 = Every single item of that type
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

                await SavePlayerInventory(chatId);
            }
            else await TelegramCommunicator.SendText(chatId, "Item " + itemString + " doesn't exist");

        }

        public static async Task<int> GetNumberOfItemsInInventory(string chatId, ObtainableItem item)
        {
            await LoadPlayerInventory(chatId);
            int numItems = 0;
            List<InventoryRecord> auxRecord = InventoryRecords.FindAll(x => x.InventoryItem.iD == item.iD); //TO-DO: Cuando se haga un refactor de los items, comprar por ID's, no por nombres
            foreach (InventoryRecord i in auxRecord)
            {
                numItems += i.Quantity;
            }

            return numItems;
        }

        public static async Task ShowInventory(string chatId)
        {
            await LoadPlayerInventory(chatId);
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
        public static async Task ShowGear(string chatId)
        {
            await LoadPlayerInventory(chatId);
            string message = "User equipment:\n";
            for (int k = 0; k < equipment.Length; k++)
            {
                message += "\n" + (EQUIPMENT_SLOT)k + ": ";
                if (equipment[k] == null) message += " empty";
                else message += equipment[k].name;
            }

            if (message != "") await TelegramCommunicator.SendText(chatId, message);
        }

        /// <summary>
        /// Shows the player's equipped items from the specified gear slot
        /// </summary>
        public static async Task ShowGear(string chatId, EQUIPMENT_SLOT slot)
        {
            await LoadPlayerInventory(chatId);
            string message = "User equipment on " + slot + " slot: ";
            if (equipment[(int)slot] == null) message += " empty";
            else message += equipment[(int)slot].name;

            if (message != "") await TelegramCommunicator.SendText(chatId, message);
        }

        /// <summary>
        /// Unequips the item worn on the specified gear slot
        /// </summary>
        public static async Task UnequipGear(string chatId, EQUIPMENT_SLOT slot)
        {
            await LoadPlayerInventory(chatId);
            if (await BattleSystem.IsPlayerInBattle(chatId))
            {
                await TelegramCommunicator.SendText(chatId, "Can't unequip your gear in battle");
            }
            else
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
                            msg += "\n" + stat.Key + " ";
                            if (stat.Value >= 0) msg += "-" + stat.Value;
                            else msg += "+" + Math.Abs(stat.Value);
                        }
                    }
                    await TelegramCommunicator.SendText(chatId, msg);

                    equipment[(int)slot] = null;

                    //Remove the item from the inventory
                    await AddItem(chatId, item.name, 1);
                    await SavePlayerInventory(chatId);
                    await BattleSystem.SavePlayerBattle(chatId);
                }
                else
                {
                    await TelegramCommunicator.SendText(chatId, "Couldn't unequip an item from your " + slot.ToString().ToLower() + " gear slot because it's empty.");
                }
            }
        }

        /// <summary>
        /// Unequips the item worn on the gear slot of the specified Equippable Item
        /// </summary>
        public static async Task UnequipGear(string chatId, EquipableItem item)
        {
            //Get the currently equipped item
            EquipableItem currentItem = await GetItemFromEquipmentSlot(chatId, item.gearSlot);

            //If there are NO items being equipped in that slot...
            if(currentItem == null) await TelegramCommunicator.SendText(chatId, "You are not wearing anything on your " + item.gearSlot + " slot");

            //If the currently equipped item is the same item you want to unequip... (This is what we expect the user to do)
            else if (currentItem.iD == item.iD) await UnequipGear(chatId, item.gearSlot);

            //If there is an item currently being equipped on that slot but it's not the requested item to unequip
            else await TelegramCommunicator.SendText(chatId, "You are not wearing that item\nDid you mean /unequip_" + currentItem.name + " ?");
        }

        /// <summary>
        /// Equips a piece of gear in its gear slot
        /// </summary>
        public static async Task EquipGear(string chatId, EquipableItem item)
        {
            //Load the player's inventory
            await LoadPlayerInventory(chatId);
            //If the player is in the middle of a battle, it can't change it's equipment
            if (await BattleSystem.IsPlayerInBattle(chatId))
            {
                await TelegramCommunicator.SendText(chatId, "Can't equip gear in battle");
            }
            //If the player is not in a battle...
            else
            {
                //Checks if the item is in the player's inventory. Parse from string
                if (!InventoryRecords.Exists(x => x.InventoryItem.iD == item.iD))
                {
                    await TelegramCommunicator.SendText(chatId, "Item " + item.name + " couldn't be equipped as it was not found in your inventory");
                }
                //If the item in in the player's inventory...
                else
                {
                    //Check if the slot is currently being occupied
                    if (equipment[(int)item.gearSlot] != null)
                    {
                        //If it's being occupied by the same item, just leave it as it is.
                        if (item.iD == equipment[(int)item.gearSlot].iD) await TelegramCommunicator.SendText(chatId, "You are already using that item");
                        //If it's being occupied, swap the gear pieces.
                        else await SwapGear(chatId, item);
                    }
                    //If the slot is free, occupy it with the new item.
                    else
                    {
                        //Equip the new item
                        item.OnEquip(chatId);

                        string msg = "You have equipped " + item.name + "on your " + item.gearSlot.ToString().ToLower() + " slot.";
                        //Apply the stat changes
                        if (item.statModifiers.Count > 0)
                        {
                            foreach (var stat in item.statModifiers)
                            {
                                msg += "\n" + stat.Key + " ";
                                if (stat.Value >= 0) msg += "+" + stat.Value;
                                else msg += stat.Value;
                            }
                        }
                        await TelegramCommunicator.SendText(chatId, msg);

                        equipment[(int)item.gearSlot] = item;

                        //Remove the item from the inventory
                        //TO-DO: Kinda jank. Currently needed because consumeItem needs to load the currentBattle. If this wasn't added, ConsumeItem would override the stat changes from the item that was just equipped.
                        await BattleSystem.SavePlayerBattle(chatId); 
                        await ConsumeItem(chatId, item.name, 1);
                    }
                    await SavePlayerInventory(chatId);
                }
            }
        }

        /// <summary>
        /// Swaps the current equipped piece of gear for a new one. Shows the change of stats.
        /// </summary>
        private static async Task SwapGear(string chatId, EquipableItem newItem)
        {            
            EquipableItem oldItem = equipment[(int)newItem.gearSlot];
            string msg = "You've swapped " + oldItem.name + " for " + newItem.name;
            Dictionary<StatName, int> auxChanges = oldItem.statModifiers.ToDictionary(x => x.Key, x => -x.Value);

            foreach (StatName sn in newItem.statModifiers.Keys)
            {
                if (oldItem.statModifiers.ContainsKey(sn))
                    auxChanges[sn] += newItem.statModifiers[sn];
                else auxChanges.Add(sn, newItem.statModifiers[sn]);
            }

            foreach (var stat in auxChanges)
            {
                msg += "\n" + stat.Key + " ";
                if (stat.Value >= 0) msg += "+" + stat.Value;
                else msg += stat.Value;
            }
            await TelegramCommunicator.SendText(chatId, msg);

            //Add the unequipped item to the inventory
            await AddItem(chatId, oldItem.name, 1);
            //Remove the item from the inventory
            await ConsumeItem(chatId, newItem.name, 1);

            equipment[(int)newItem.gearSlot].OnUnequip(chatId);
            equipment[(int)newItem.gearSlot] = newItem;
            equipment[(int)newItem.gearSlot].OnEquip(chatId);
            await BattleSystem.SavePlayerBattle(chatId);
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

            public Dictionary<string, object> GetSerializable()
            {
                return new Dictionary<string, object>
                {
                    {InventoryItem.name, Quantity}
                };
            }

        }
    }
}
