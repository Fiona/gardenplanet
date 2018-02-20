using System.Collections;
using Rewired;
using StompyBlondie;
using UnityEngine;

namespace StrawberryNova
{
    public class InGameMenuPageQuitGame: MonoBehaviour, IInGameMenuPage
    {
        [Header("Settings")]
        public string displayName = "Quit Game";
        public GlobalButton toTitleButton;
        public GlobalButton toDesktopButton;
        private GameController controller;

        private void Awake()
        {
            controller = FindObjectOfType<GameController>();
            toTitleButton.SetCallback(() => StartCoroutine(TitleButtonPressed()));
            toDesktopButton.SetCallback(() => StartCoroutine(DesktopButtonPressed()));
        }

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

        private IEnumerator TitleButtonPressed()
        {
            yield return controller.screenFade.FadeOut(3f,
                callback: () => FindObjectOfType<App>().StartNewState(AppState.Title));
        }

        private IEnumerator DesktopButtonPressed()
        {
            yield return controller.screenFade.FadeOut(3f, callback: FindObjectOfType<App>().Quit);
        }
    }
}