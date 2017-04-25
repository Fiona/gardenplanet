using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StrawberryNova
{
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

        public bool GivePlayerItem(ItemType itemType, Hashtable attributes = null, int quantity = 1)
        {
            if(itemType == null)
                return false;
            if(attributes == null)
                attributes = new Hashtable(itemType.Attributes);
            return controller.player.inventory.AddItem(itemType, attributes, quantity);
        }

        public bool GivePlayerItem(string itemTypeID, Hashtable attributes = null, int quantity = 1)
        {
            return GivePlayerItem(GetItemTypeByID(itemTypeID), attributes, quantity);
        }

        public bool RemovePlayerItem(ItemType itemType, Hashtable attributes, int quantity = 1)
        {
            return controller.player.inventory.RemoveItem(itemType, attributes, quantity);
        }

        public bool RemovePlayerItem(string itemTypeID, Hashtable attributes, int quantity = 1)
        {
            return RemovePlayerItem(GetItemTypeByID(itemTypeID), attributes, quantity);
        }

        public ItemType GetItemTypeByID(string itemTypeID)
        {
            foreach(var type in itemTypes)
                if(type.ID == itemTypeID)
                    return type;
            return null;
        }

    }
}
    