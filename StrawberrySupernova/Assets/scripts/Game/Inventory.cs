using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace StrawberryNova
{
    public class Inventory
    {
        
        public int maximumItemStacks;

        public class InventoryItemEntry
        {
            public ItemType itemType;
            public int quantity;
            public Hashtable attributes;
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
        public bool AddItem(ItemType itemType, Hashtable attributes, int quantity = 1)
        {
            
            // Check if we have any stacks of this type
            var existingEntries = GetItemEntriesOfType(itemType, attributes);

            // If we don't have any add a new one
            if(existingEntries.Count == 0)
            {
                // Check we haven't accidently got to the inventory limit
                if(items.Count >= maximumItemStacks)
                    return false;
                // Okay to start a new stack
                items.Add(
                    new InventoryItemEntry {
                        itemType = itemType,
                        quantity = quantity,
                        attributes = new Hashtable(attributes)
                    }
                );
                return true;
            }

            // We have existing entries, let's see if we can fit in one of them
            foreach(var entry in existingEntries)
            {
                // If we can fit in this stack then use it
                if(entry.quantity + quantity <= itemType.StackSize)
                {
                    entry.quantity += quantity;
                    return true;
                }
            }

            // If we got here then none of the existing stacks were appropriate
            // so we check if we have room for a new stack afterall.
            if(items.Count < maximumItemStacks)
            {
                items.Add(
                    new InventoryItemEntry
                    {
                        itemType = itemType,
                        quantity = quantity,
                        attributes = new Hashtable(attributes)
                    }
                );
                return true;
            }

            // The item doesn't fit into the inventory at all
            return false;

        }

        /*
         * Removes one or more item types from the inventory.
         * Return false if the item wasn't there or true otherwise.
         */
        public bool RemoveItem(ItemType itemType, Hashtable attributes, int quantity = 1)
        {            
            var entries = GetItemEntriesOfType(itemType, attributes);
            // We don't have that type in the inventory
            if(entries.Count == 0)
                return false;

            // Reduce by amount
            entries[0].quantity -= quantity;

            // Destroy any entries that no longer have an item in
            var cloneList = new List<InventoryItemEntry>(items);
            foreach(var entry in cloneList)
                if(entry.quantity <= 0)
                    items.Remove(entry);
            return true;
        }

        List<InventoryItemEntry> GetItemEntriesOfType(ItemType itemType, Hashtable attributes)
        {
            var list = new List<InventoryItemEntry>();
            foreach(var item in items)
                if(item.itemType == itemType && DoAttributesMatch(attributes, item.attributes))
                        list.Add(item);
            return list;
        }

        public bool DoAttributesMatch(Hashtable attributesA, Hashtable attributesB)
        {
            foreach(var key in attributesA.Keys)
                if(!attributesB.ContainsKey(key) || attributesA[key] != attributesB[key])
                    return false;
            return true;
        }

    }
}

