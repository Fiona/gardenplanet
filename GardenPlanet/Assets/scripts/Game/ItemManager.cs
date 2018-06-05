using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GardenPlanet
{
    /*
     * Holds all the item types currently loaded and can be used to give or remove items from the player
     * inventory.
     */
    public class ItemManager: MonoBehaviour
    {

        public List<ItemType> itemTypes;
        GameController controller;

        public void Awake()
        {
            itemTypes = ItemType.GetAllItemTypes();
        }

        public void Start()
        {
            controller = FindObjectOfType<GameController>();
        }

        /*
         * Attempts to give a player an item matching the passed ItemType object and attrbutes.
         * Returns true if the player has room in their inventory for such an item and it was added
         * successfully.
         */
        public bool GivePlayerItem(ItemType itemType, Attributes attributes = null, int quantity = 1)
        {
            if(controller == null)
                controller = FindObjectOfType<GameController>();
            if(itemType == null)
                return false;
            if(attributes == null)
                attributes = new Attributes(itemType.Attributes);
            return controller.player.inventory.AddItem(itemType, attributes, quantity);
        }

        /*
         * Attempts to give a player an item matching the passed ID string and attrbutes.
         * Returns true if the player has room in their inventory for such an item, the item exists and
         * that it was added successfully.
         */
        public bool GivePlayerItem(string itemTypeID, Attributes attributes = null, int quantity = 1)
        {
            return GivePlayerItem(GetItemTypeByID(itemTypeID), attributes, quantity);
        }

        /*
         * Attempts to remove an item matching the passed ItemType object and attributes.
         * Returns true if the player has such an item and it was removed successfully.
         */
        public bool RemovePlayerItem(ItemType itemType, Attributes attributes, int quantity = 1)
        {
            return controller.player.inventory.RemoveItem(itemType, attributes, quantity);
        }

        /*
         * Attempts to remove an item matching the passed ID string and attributes.
         * Returns true if the player has such an item and it was removed successfully.
         */
        public bool RemovePlayerItem(string itemTypeID, Attributes attributes, int quantity = 1)
        {
            return RemovePlayerItem(GetItemTypeByID(itemTypeID), attributes, quantity);
        }

        /*
         * Attempts to remove an item matching the passed inventory entry.
         * Returns true if the player has such an item and it was removed successfully.
         */
        public bool RemovePlayerItem(Inventory.InventoryItemEntry entry, int quantity = 1)
        {
            return controller.player.inventory.RemoveItem(entry, quantity);
        }

        /*
         * Quick method to get a recoginsed ItemType object whose ID matches the passed string.
         * Returns the ItemType or null.;
         */
        public ItemType GetItemTypeByID(string itemTypeID)
        {
            foreach(var type in itemTypes)
                if(type.ID == itemTypeID)
                    return type;
            return null;
        }

    }
}
