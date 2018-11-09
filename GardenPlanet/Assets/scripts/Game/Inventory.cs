using System;
using System.Collections.Generic;
using System.Linq;

namespace GardenPlanet
{
    public class Inventory
    {

        public int maximumItemStacks;

        public class InventoryItemEntry
        {
            public ItemType itemType;
            public int quantity;
            public Attributes attributes;

            public List<KeyValuePair<string, string>> GetAttributeDisplay()
            {
                var list = new List<KeyValuePair<string, string>>();
                list.Add(new KeyValuePair<string, string>("Category", itemType.Category));
                return list;
            }

            public override string ToString()
            {
                return $"[type: {itemType.DisplayName}, num: {quantity}, attrs: {attributes}]";
            }
        }

        public List<InventoryItemEntry> Items
        {
            get
            {
                return new List<InventoryItemEntry>(items);
            }
        }

        List<InventoryItemEntry> items;

        public Inventory(int maxItemStacks)
        {
            maximumItemStacks = maxItemStacks;
            items = new List<InventoryItemEntry>();
        }

        /*
         * Used to add one or more items of a single type to the inventory.
         * Returns true if successful or false if the item wouldn't fit into
         * the inventory.
         */
        public bool AddItem(ItemType itemType, Attributes attributes, int quantity = 1)
        {
            if(!ItemAddSuccessProbe(itemType, attributes, quantity))
                return false;
            return DoItemAdd(itemType, attributes, quantity, items);
        }

        /*
         * Removes one or more items of a passed type from the inventory.
         * Return false if there weren't enough of the item in the inventory or true otherwise.
         */
        public bool RemoveItem(ItemType itemType, Attributes attributes, int quantity = 1)
        {
            if(!ItemRemoveSuccessProbe(itemType, attributes, quantity))
                return false;
            return DoItemRemove(itemType, attributes, quantity, items);
        }

        /*
         * Removes one or more of the entry passed. Returns false if the entry doesn't exist
         * or there aren't enough of the item on that entry
         */
        public bool RemoveItem(InventoryItemEntry entry, int quantity = 1)
        {
            if(!ItemEntryExists(entry))
                return false;
            if(entry.quantity < quantity)
                return false;
            entry.quantity -= quantity;
            if(entry.quantity <= 0)
                items.Remove(entry);
            return true;
        }

        public bool ItemEntryExists(InventoryItemEntry entry)
        {
            return items.IndexOf(entry) > -1;
        }

        List<InventoryItemEntry> GetItemEntriesOfType(ItemType itemType, Attributes attributes,
            List<InventoryItemEntry> itemList = null)
        {
            if(itemList == null)
                itemList = items;
            var list = new List<InventoryItemEntry>();
            foreach(var item in itemList)
                if(item.itemType == itemType && attributes == item.attributes)
                    list.Add(item);
            return list;
        }

        bool ItemAddSuccessProbe(ItemType itemType, Attributes attributes, int quantity)
        {
            return DoItemAdd(itemType, attributes, quantity, CloneItemList(items));
        }

        bool DoItemAdd(ItemType itemType, Attributes attributes, int quantity, List<InventoryItemEntry> itemList)
        {
            // We managed to add everything
            if(quantity <= 0)
                return true;

            // Try and get existing stacks with room, we try to fill those first
            var existingStacks = GetItemEntriesOfType(itemType, attributes, itemList)
                .Where(x => x.quantity < itemType.StackSize).ToList();

            if(existingStacks.Count > 0)
            {
                var spaceRemaining = itemType.StackSize - existingStacks[0].quantity;

                // Only add up to the remaining space or whole quantity depending on how much
                // is left
                var quantityToAdd = quantity <= spaceRemaining ? quantity : spaceRemaining;
                existingStacks[0].quantity += quantityToAdd;

                // Go round again fitting the rest in
                return DoItemAdd(itemType, attributes, quantity - quantityToAdd, itemList);
            }

            // If there's room for another stack then add it
            if(maximumItemStacks > itemList.Count)
            {
                var quantityToAdd = quantity < itemType.StackSize ? quantity : itemType.StackSize;
                itemList.Add(new InventoryItemEntry{
                    itemType = itemType,
                    attributes = new Attributes(attributes),
                    quantity = quantityToAdd
                });
                // Recurse, trying to fit remaining in
                return DoItemAdd(itemType, attributes, quantity - quantityToAdd, itemList);
            }

            return false;
        }

        bool ItemRemoveSuccessProbe(ItemType itemType, Attributes attributes, int quantity)
        {
            return DoItemRemove(itemType, attributes, quantity, CloneItemList(items));
        }

        bool DoItemRemove(ItemType itemType, Attributes attributes, int quantity, List<InventoryItemEntry> itemList)
        {
            // We managed to remove everything
            if(quantity <= 0)
                return true;
            // Try and get existing stacks, we try to take from those first
            var existingStacks = GetItemEntriesOfType(itemType, attributes, itemList)
                .OrderBy(x => x.quantity).ToList();
            // If none exist then we can't remove them
            if(existingStacks.Count == 0)
                return false;
            // See how many we can take from the first stack
            var quantityToRemove = quantity <= existingStacks[0].quantity ? quantity : existingStacks[0].quantity;
            // Remove items
            existingStacks[0].quantity -= quantityToRemove;
            // Destroy entry if empty
            if(existingStacks[0].quantity <= 0)
                itemList.Remove(existingStacks[0]);
            // Recurse, trying to remove the remaining quantity
            return DoItemRemove(itemType, attributes, quantity - quantityToRemove, itemList);
        }

        List<InventoryItemEntry> CloneItemList(List<InventoryItemEntry> toClone)
        {
            var newItemList = new List<InventoryItemEntry>();
            foreach(var curItem in items)
            {
                newItemList.Add(
                    new InventoryItemEntry
                    {
                        itemType = curItem.itemType,
                        quantity = curItem.quantity,
                        attributes = new Attributes(curItem.attributes)
                    }
                );
            }
            return newItemList;
        }

    }
}