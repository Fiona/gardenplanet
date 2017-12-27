using System.Collections;
using System.Linq;

namespace StrawberryNova
{
    namespace Items
    {
        public class WateringCan : ItemScript
        {
            public override bool CanBeUsedOnTilePos(TilePosition tilePos)
            {
                if(tilePos.TileDistance(controller.player.CurrentTilePosition) >= Consts.PLAYER_TOOLS_RANGE)
                    return false;

                return tilePos.GetTileWorldObjects("crop").Count > 0;
            }

            public override IEnumerator UseOnTilePos(TilePosition tilePos)
            {
                var crop = tilePos.GetTileWorldObjects("crop")[0];
                crop.attributes["watered"] = true;
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