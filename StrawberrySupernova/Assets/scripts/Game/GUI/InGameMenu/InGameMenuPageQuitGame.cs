using System.Collections;
using StompyBlondie;
using UnityEngine;

namespace StrawberryNova
{
    public class InGameMenuPageQuitGame: MonoBehaviour, IInGameMenuPage
    {
        [Header("Settings")]
        public string displayName = "Quit Game";

        public string GetDisplayName()
        {
            return displayName;
        }

        public IEnumerator Open()
        {
            gameObject.SetActive(true);
            yield return LerpHelper.QuickFadeIn(GetComponent<CanvasGroup>(), Consts.GUI_IN_GAME_MENU_PAGE_FADE_TIME,
                LerpHelper.Type.SmoothStep);
        }

        public IEnumerator Close()
        {
            yield return LerpHelper.QuickFadeOut(GetComponent<CanvasGroup>(), Consts.GUI_IN_GAME_MENU_PAGE_FADE_TIME,
                LerpHelper.Type.SmoothStep);
            gameObject.SetActive(false);
        }

        public void Input(GameInputManager inputManager)
        {
            return;
        }
    }
}