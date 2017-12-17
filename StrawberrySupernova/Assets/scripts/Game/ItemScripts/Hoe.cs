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
                // Check the tile is hoeable, that it's close enough and that it doesn't contain colliding world objects 
                var initialCheck = controller.tileTagManager.IsTileTaggedWith(tilePos, Consts.TILE_TAG_FARM) &&
                                   tilePos.TileDistance(controller.player.CurrentTilePosition) < Consts.PLAYER_TOOLS_RANGE &&
                                   !tilePos.ContainsCollidableWorldObjects();
                // Any of those are an immediate no-no
                if(!initialCheck)
                    return false;

                // Get any tile objects on there so we can check for crops and hoed ground because we can
                // use it on those to destroy them.
                var tileObjects = tilePos.GetTileWorldObjects();
                if(tileObjects.Count > 0)
                {
                    foreach(var tileObject in tileObjects)
                        if(tileObject.name == "hoedsoil")
                            return true;
                    // must be something else, so let's say no
                    return false;
                }
                
                // Definitely nothing there so yep
                return true;
            }

            public IEnumerator UseOnTilePos(TilePosition tilePos)
            {
                // Check for hoed soil, using the hoe on it causes it to be removed
                var tileObjects = tilePos.GetTileWorldObjects();
                if(tileObjects.Count > 0)
                {
                    foreach(var tileObject in tileObjects)
                    {
                        if(tileObject.name == "hoedsoil")
                        {
                            controller.worldObjectManager.DeleteWorldObject(tileObject);
                            yield break;
                        }
                    }
                }

                // Empty tile so hoe the ground
                var soil = controller.worldObjectManager.AddWorldObject(
                    controller.worldObjectManager.GetWorldObjectTypeByName("hoedsoil"), tilePos
                );
                soil.gameObject.transform.localRotation = Quaternion.Euler(0f, (float)UnityEngine.Random.Range(0, 360), 0f);
                yield break;
            }

            public bool CanBeDroppedOnTilePos(TilePosition tilePos)
            {
                return true;
            }
        }
    }
}