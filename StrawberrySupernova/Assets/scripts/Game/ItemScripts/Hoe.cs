using System;
using System.Collections;
using UnityEngine;

namespace StrawberryNova
{
    namespace Items
    {
        public class Hoe : MonoBehaviour, IItemScript
        {
            GameController controller;

            public void StartsHolding(Inventory.InventoryItemEntry item)
            {
                controller = FindObjectOfType<GameController>();
                Debug.Log("Holding hoe");
            }

            public bool CanBeUsedOnTilePos(TilePosition tilePos)
            {
                return controller.tileTagManager.IsTileTaggedWith(tilePos, Consts.TILE_TAG_FARM) &&
                       tilePos.TileDistance(controller.player.CurrentTilePosition) < Consts.PLAYER_TOOLS_RANGE &&
                    !tilePos.ContainsWorldObjects();
            }

            public IEnumerator UseOnTilePos(TilePosition tilePos)
            {
                Debug.Log("Hoe!");
                yield break;
            }

            public bool CanBeDroppedOnTilePos(TilePosition tilePos)
            {
                return true;
            }
        }
    }
}