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
            public override bool CanBeUsedOnTilePos(TilePosition tilePos)
            {
                if(tilePos.TileDistance(controller.player.CurrentTilePosition) >= Consts.PLAYER_TOOLS_RANGE)
                    return false;

                return tilePos.GetTileWorldObjects("crop").Any(crop => crop.GetAttrString("type") == "");
            }

            public override IEnumerator UseOnTilePos(TilePosition tilePos)
            {
                var crop = tilePos.GetTileWorldObjects("crop")[0];
                crop.attributes["type"] = item.attributes["type"];
                crop.SetAppearence();
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