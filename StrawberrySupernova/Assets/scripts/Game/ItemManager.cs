using System;
using System.Collections.Generic;
using UnityEngine;

namespace StrawberryNova
{
    public class ItemManager: MonoBehaviour
    {

        public List<ItemType> itemTypes;

        public void Awake()
        {
            itemTypes = ItemType.GetAllItemTypes();
        }

    }
}

