using System.Collections;

namespace GardenPlanet
{
    namespace Items
    {
        public class WateringCan : ItemScript
        {
            public override bool IsTileItem()
            {
                return true;
            }

            public override bool CanBeUsedOnTilePos(TilePosition tilePos)
            {
                if(tilePos.TileDistance(controller.player.CurrentTilePosition) >= Consts.PLAYER_TOOLS_RANGE)
                    return false;

                var crops = tilePos.GetTileWorldObjects("crop");
                if(crops.Count == 0 || crops[0].attributes.Get<float>("growth") >= 100f)
                    return false;
                return true;
            }

            public override IEnumerator UseOnTilePos(TilePosition tilePos)
            {
                // Make sure we reduced energy
                var energyConsumption = controller.globalConfig.energyUsage["wateringCan"] *
                                        item.attributes.Get<float>("energyConsumptionModifier");
                if(!controller.ConsumePlayerEnergy(energyConsumption))
                    yield break;

                var crop = tilePos.GetTileWorldObjects("crop")[0];
                crop.attributes.Set("watered", true);
                crop.SetAppearence();
                yield return null;
            }

            public override bool CanBeDroppedOnTilePos(TilePosition tilePos)
            {
                return true;
            }
        }
    }
}