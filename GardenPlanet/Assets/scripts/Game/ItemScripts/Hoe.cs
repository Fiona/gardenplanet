using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StompyBlondie;
using UnityEngine;

namespace GardenPlanet
{
    namespace Items
    {
        public class Hoe : ItemScript
        {
            public override bool IsTileItem()
            {
                return true;
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
                // Crops do special things (like removing hoed soil, getting seeds back, etc)
                if(tilePos.GetTileWorldObjects("crop").Count > 0)
                    return true;
                // Can't use if something in the way
                if(tilePos.GetTileWorldObjects().Count > 0)
                    return false;
                // Definitely nothing there so yep
                return true;
            }

            public override IEnumerator UseOnTilePos(TilePosition tilePos)
            {
                // Make sure we reduced energy
                var energyConsumption = controller.globalConfig.energyUsage["hoe"] *
                                        item.attributes.Get<float>("energyConsumptionModifier");
                if(!controller.ConsumePlayerEnergy(energyConsumption))
                    yield break;

                // Special things happen on hoed and seeded soil
                var tileObjects = tilePos.GetTileWorldObjects("crop");
                if(tileObjects.Count > 0)
                {
                    foreach(var i in tileObjects)
                    {
                        var cropType = i.attributes.Get<string>("type");
                        // Unseeded soil is removed
                        if(cropType == "")
                        {
                            controller.worldObjectManager.DeleteWorldObject(i);
                            controller.autoTileManager.UpdateTilesSurrounding(new TilePosition(i.GetWorldPosition()));
                            yield break;
                        }
                        // Ungrown seeds can be fished out
                        if(i.attributes.Get<float>("growth") < 1f)
                        {
                            controller.GivePlayerItem(
                                i.attributes.Get<string>("type")+"_seeds",
                                new Attributes{{"type", cropType}}
                                );
                            controller.worldObjectManager.DeleteWorldObject(i);
                            controller.autoTileManager.UpdateTilesSurrounding(new TilePosition(i.GetWorldPosition()));
                            yield break;
                        }
                        // Damage part grown crops and destroy if broken
                        var effect = Instantiate(Resources.Load("effects/oneshot/CropDamage")) as GameObject;
                        effect.transform.position = i.gameObject.transform.position;
                        var damage = i.attributes.Get<float>("damage") + Consts.CROP_DAMAGE_PER_HOE_HIT;
                        i.attributes.Set("damage", damage);
                        if(damage >= 100f)
                            controller.worldObjectManager.DeleteWorldObject(i);
                        yield break;
                    }
                }

                // Empty tile so hoe the ground
                controller.worldObjectManager.AddWorldObject(
                    controller.worldObjectManager.GetWorldObjectTypeByName("crop"),
                    tilePos
                );
            }

            public override bool CanBeDroppedOnTilePos(TilePosition tilePos)
            {
                return true;
            }
        }
    }
}