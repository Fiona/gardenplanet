using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StompyBlondie;
using UnityEngine;

namespace StrawberryNova
{
    namespace Items
    {
        public class Hoe : ItemScript
        {
            GameController controller;

            public override void StartsHolding()
            {
                controller = FindObjectOfType<GameController>();
            }

            public override bool CanBeUsedOnTilePos(TilePosition tilePos)
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
                var crops = tilePos.GetTileWorldObjects("crop");
                if(crops.Count > 0)
                    if(crops.Any(crop => crop.GetAttrString("type") == ""))
                        return true;
                // Can't use if something in the way
                if(tilePos.GetTileWorldObjects().Count > 0)
                    return false;
                // Definitely nothing there so yep
                return true;
            }

            public override IEnumerator UseOnTilePos(TilePosition tilePos)
            {
                // Check for hoed soil, it can be unhoed if empty
                var tileObjects = tilePos.GetTileWorldObjects("crop");
                if(tileObjects.Count > 0)
                {
                    foreach(var i in tileObjects)
                        if(i.GetAttrString("type") == "")
                            controller.worldObjectManager.DeleteWorldObject(i);
                    yield break;
                }

                // Empty tile so hoe the ground
                var soil = controller.worldObjectManager.AddWorldObject(
                    controller.worldObjectManager.GetWorldObjectTypeByName("crop"), tilePos
                );
                soil.gameObject.transform.localRotation = Quaternion.Euler(0f, (float)UnityEngine.Random.Range(0, 360), 0f);
            }

            public override bool CanBeDroppedOnTilePos(TilePosition tilePos)
            {
                return true;
            }
        }
    }
}