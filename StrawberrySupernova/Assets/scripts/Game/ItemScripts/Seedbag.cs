using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StompyBlondie;
using UnityEngine;
using UnityEngine.AI;

namespace StrawberryNova
{
    namespace Items
    {
        public class Seedbag : ItemScript
        {
            GameController controller;

            public override void StartsHolding()
            {
                controller = FindObjectOfType<GameController>();
            }

            public override bool CanBeUsedOnTilePos(TilePosition tilePos)
            {
                if(tilePos.TileDistance(controller.player.CurrentTilePosition) >= Consts.PLAYER_TOOLS_RANGE)
                    return false;

                return tilePos.GetTileWorldObjects("hoedsoil").Count > 0;
            }

            public override IEnumerator UseOnTilePos(TilePosition tilePos)
            {
                // Get soil rotation
                var soil = tilePos.GetTileWorldObjects("hoedsoil")[0];
                var rotation = soil.gameObject.transform.localRotation;

                // Drop seeds on top
                var type = controller.worldObjectManager.GetWorldObjectTypeByName("crop");
                var seedling = controller.worldObjectManager.AddWorldObject(
                    type,
                    tilePos,
                    new Hashtable()
                    {
                        {"type", item.attributes["type"]}
                    }
                );
                seedling.gameObject.transform.localRotation = rotation;

                controller.RemovePlayerItem(item.itemType, item.attributes, 1);
                yield return null;
            }

            public override bool CanBeDroppedOnTilePos(TilePosition tilePos)
            {
                return true;
            }
        }
    }
}