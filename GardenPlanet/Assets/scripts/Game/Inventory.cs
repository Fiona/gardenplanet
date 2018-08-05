﻿using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using LitJson;
using UnityEngine;

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
                        attributes = new Attributes(attributes)
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
                        attributes = new Attributes(attributes)
                    }
                );
                return true;
            }

            // The item doesn't fit into the inventory at all
            return false;

        }

        /*
         * Removes one or more items of a passed type from the inventory. Will always
         * attempt to remove up to the quantity given.
         * Return false if the item wasn't there or true otherwise.
         */
        public bool RemoveItem(ItemType itemType, Attributes attributes, int quantity = 1)
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

        List<InventoryItemEntry> GetItemEntriesOfType(ItemType itemType, Attributes attributes)
        {
            var list = new List<InventoryItemEntry>();
            foreach(var item in items)
            {
                if(item.itemType == itemType && attributes == item.attributes)
                    list.Add(item);
            }
            return list;
        }

        public bool ItemEntryExists(InventoryItemEntry entry)
        {
            return items.IndexOf(entry) > -1;
        }

    }
}