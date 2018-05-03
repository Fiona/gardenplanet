using System.Collections;
using StompyBlondie;
using UnityEngine;

namespace GardenPlanet
{
    public class InGameMenuPageCloseMenu: MonoBehaviour, IInGameMenuPage
    {
        [Header("Settings")]
        public string displayName = "Close Menu";

        public string GetPagePrefabName()
        {
            return "CloseMenu";
        }

        public string GetDisplayName()
        {
            return displayName;
        }

        public IEnumerator Open()
        {
            yield return new WaitForSeconds(.25f);
            FindObjectOfType<InGameMenu>().CloseMenu();
        }

        public IEnumerator Close()
        {
            yield break;
        }

        public void Input(GameInputManager inputManager)
        {
            return;
        }

    }
}