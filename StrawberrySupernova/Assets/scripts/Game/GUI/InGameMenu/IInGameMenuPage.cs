using System.Collections;
using UnityEngine;

namespace GardenPlanet
{
    // All in-game menu tab pages inherit from this
    public interface IInGameMenuPage
    {
        // Returns a string with the display name
        string GetDisplayName();

        // Called to open the menu, should do any animations and make it active
        IEnumerator Open();

        // Called to close the menu and do any animations and deactivate it
        IEnumerator Close();

        // Called every frame to get input
        void Input(GameInputManager inputManager);
    }
}