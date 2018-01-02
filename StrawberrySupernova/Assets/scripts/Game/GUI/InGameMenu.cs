using System;
using UnityEngine;
using System.Collections;
using StompyBlondie;

namespace StrawberryNova
{
    public class InGameMenu: MonoBehaviour
    {

        public InGameMenuInventory inventory;

        bool closeMenu;

        public IEnumerator OpenMenu()
        {
            closeMenu = false;
            yield return OpenInventoryMenu();
        }

        public IEnumerator OpenInventoryMenu()
        {
            inventory.Open();

            // Fade in
            foreach(var val in LerpHelper.LerpOverTime(Consts.GUI_IN_GAME_MENU_FADE_TIME))
            {
                GetComponent<CanvasGroup>().alpha = Mathf.Lerp(0f, 1f, val);
                yield return new WaitForFixedUpdate();
            }

            while(!closeMenu)
            {
                if(Input.GetKeyUp(KeyCode.Period) || Input.GetKeyUp(KeyCode.Escape))
                    break;
                yield return new WaitForFixedUpdate();
            }

            // Fade out
            foreach(var val in LerpHelper.LerpOverTime(Consts.GUI_IN_GAME_MENU_FADE_TIME))
            {
                GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1f, 0f, val);
                yield return new WaitForFixedUpdate();
            }
        }

        public void CloseMenuButtonPressed()
        {
            closeMenu = true;
        }

    }
}

