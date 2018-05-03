using System;
using System.Collections;
using UnityEngine;

namespace GardenPlanet
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

        // When checking if the item in hand is an item that you use on tiles or just in-hand
        public virtual bool IsTileItem()
        {
            return false;
        }

        // Called if not a tile item and checking if it can be used at all
        public virtual bool CanBeUsed()
        {
            return false;
        }

        // Called if using the item in-place in-hand
        public virtual IEnumerator Use()
        {
            yield return null;
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