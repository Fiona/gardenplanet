using System;
using System.Collections;
using UnityEngine;

namespace StrawberryNova
{
    /*
     * All scripts must implement this
     */
    public abstract class ItemScript: MonoBehaviour
    {
        public Inventory.InventoryItemEntry item;
        public GameController controller;

        // Called when the Item is put in the player's hand and the script is started.
        // Use for initialisation.
        public virtual void StartsHolding()
        {

        }

        // Called when the game wants to check if this item is usable at a particular position
        public virtual bool CanBeUsedOnTilePos(TilePosition tilePos)
        {
            return false;
        }

        // Called when the item is to be used on a particular tile
        public virtual IEnumerator UseOnTilePos(TilePosition tilePos)
        {
            yield return null;
        }

        // Called when the game wants to attempt to drop the item
        public virtual bool CanBeDroppedOnTilePos(TilePosition tilePos)
        {
            return false;
        }
    }
}
