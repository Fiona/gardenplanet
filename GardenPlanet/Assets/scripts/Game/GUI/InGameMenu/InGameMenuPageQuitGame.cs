using System.Collections;
using UnityEngine;
using StompyBlondie.Utils;

namespace GardenPlanet
{
    public class InGameMenuPageQuitGame: MonoBehaviour, IInGameMenuPage
    {
        [Header("Settings")]
        public string displayName = "Quit Game";
        public GlobalButton toTitleButton;
        public GlobalButton toDesktopButton;

        private GameController controller;
        private bool pressed;
        private GUINavigator navigator;

        private void Awake()
        {
            controller = FindObjectOfType<GameController>();
            toTitleButton.SetCallback(() => StartCoroutine(TitleButtonPressed()));
            toDesktopButton.SetCallback(() => StartCoroutine(DesktopButtonPressed()));

            if(navigator == null)
                navigator = gameObject.AddComponent<GUINavigator>();
            navigator.ClearNavigationElements();
            navigator.direction = GUINavigator.GUINavigatorDirection.Vertical;
            navigator.AddNavigationElement(toTitleButton.GetComponent<RectTransform>());
            navigator.AddNavigationElement(toDesktopButton.GetComponent<RectTransform>());
            navigator.active = true;
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
            yield return controller.screenFade.FadeOut(
                callback: () => FindObjectOfType<App>().StartNewState(AppState.Title)
            );
        }

        private IEnumerator DesktopButtonPressed()
        {
            yield return controller.screenFade.FadeOut(
                callback: FindObjectOfType<App>().Quit
            );
        }
    }
}