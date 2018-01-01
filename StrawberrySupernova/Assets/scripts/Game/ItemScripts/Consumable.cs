﻿using UnityEngine;
using System.Collections;

namespace StrawberryNova.Items
{
    public class Consumable: ItemScript
    {
        public override bool CanBeUsed()
        {
            return true;
        }

        public override IEnumerator Use()
        {
            controller.IncreasePlayerEnergy(item.GetAttrFloat("energy_increase"));
            controller.RemovePlayerItem(item.itemType, item.attributes, 1);
            yield return null;
        }
    }
}