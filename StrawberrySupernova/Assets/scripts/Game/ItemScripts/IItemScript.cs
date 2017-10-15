using System.Collections;

namespace StrawberryNova
{
    /*
     * All scripts must implement this
     */
    public interface IItemScript
    {
        // Called when the Item is put in the player's hand and the script is started.
        // Use for initialisation
        void StartsHolding();

        // Called when the game wants to check if this item is usable at a particular position
        bool CanBeUsedOnTilePos(TilePosition tilePos);
        
        // Called when the item is to be used on a particular tile
        IEnumerator UseOnTilePos(TilePosition tilePos);        
    }
}
