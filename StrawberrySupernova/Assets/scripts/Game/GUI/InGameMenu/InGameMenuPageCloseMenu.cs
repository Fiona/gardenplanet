using System.Collections;
using StompyBlondie;
using UnityEngine;

namespace StrawberryNova
{
    public class InGameMenuPageCloseMenu: MonoBehaviour, IInGameMenuPage
    {
        [Header("Settings")]
        public string displayName = "Close Menu";

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
    }
}