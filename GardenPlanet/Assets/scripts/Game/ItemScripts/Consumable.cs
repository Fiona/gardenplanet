using UnityEngine;
using System.Collections;

namespace GardenPlanet.Items
{
    public class Consumable: ItemScript
    {
        public override bool CanBeUsed()
        {
            return true;
        }

        public override IEnumerator Use()
        {
            yield return StartCoroutine(controller.PlayerDoEat());
            controller.IncreasePlayerEnergy(item.attributes.Get<float>("energyIncrease"));
            controller.RemovePlayerItem(item.itemType, item.attributes, 1);
            controller.itemHotbar.UpdateItemInHand();
            yield return null;
        }
    }
}