using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StompyBlondie;
using UnityEngine;
using UnityEngine.AI;

namespace GardenPlanet
{
    namespace Items
    {
        public class Seedbag : ItemScript
        {
            public override bool IsTileItem()
            {
                return true;
            }

            public override bool CanBeUsedOnTilePos(TilePosition tilePos)
            {
                if(tilePos.TileDistance(controller.player.CurrentTilePosition) >= Consts.PLAYER_TOOLS_RANGE)
                    return false;

                return tilePos.GetTileWorldObjects("crop").Any(crop => crop.attributes.Get<string>("type") == "");
            }

            public override IEnumerator UseOnTilePos(TilePosition tilePos)
            {
                // Make sure we reduced energy
                var energyConsumption = controller.globalConfig.energyUsage["seedbag"];
                if(!controller.ConsumePlayerEnergy(energyConsumption))
                    yield break;

                var crop = tilePos.GetTileWorldObjects("crop")[0];
                crop.attributes.Set("type", item.attributes.Get<string>("type"));
                crop.SetAppearence();
                controller.RemovePlayerItem(item, 1);
                yield return null;
            }

            public override bool CanBeDroppedOnTilePos(TilePosition tilePos)
            {
                return true;
            }
        }
    }
}