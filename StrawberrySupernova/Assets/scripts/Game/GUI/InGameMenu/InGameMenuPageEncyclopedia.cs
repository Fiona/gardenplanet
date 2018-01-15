using System.Collections;
using StompyBlondie;
using UnityEngine;

namespace StrawberryNova
{
    public class InGameMenuPageEncyclopedia: MonoBehaviour, IInGameMenuPage
    {
        [Header("Settings")]
        public string displayName = "Encyclopedia";

        public string GetDisplayName()
        {
            return displayName;
        }

        public IEnumerator Open()
        {
            gameObject.SetActive(true);
            yield return StartCoroutine(
                LerpHelper.QuickFadeIn(GetComponent<CanvasGroup>(), Consts.GUI_IN_GAME_MENU_PAGE_FADE_TIME)
                );
        }

        public IEnumerator Close()
        {
            yield return StartCoroutine(
                LerpHelper.QuickFadeOut(GetComponent<CanvasGroup>(), Consts.GUI_IN_GAME_MENU_PAGE_FADE_TIME)
                );
            gameObject.SetActive(false);
        }
    }
}