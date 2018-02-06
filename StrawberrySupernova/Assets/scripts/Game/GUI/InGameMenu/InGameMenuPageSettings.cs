using System.Collections;
using System.Net.Sockets;
using StompyBlondie;
using UnityEngine;

namespace StrawberryNova
{
    public class InGameMenuPageSettings: MonoBehaviour, IInGameMenuPage
    {

        [Header("Settings")]
        public string displayName = "Settings";

        [Header("Object references")]
        public SettingsCategory[] categories;

        [HideInInspector]
        public bool openingPage;

        private SettingsCategory openCategory;

        public string GetDisplayName()
        {
            return displayName;
        }

        public IEnumerator Open()
        {
            gameObject.SetActive(true);

            var canvas = GetComponent<CanvasGroup>();
            canvas.alpha = 0f;

            foreach(var cat in categories)
                cat.Init();

            yield return LerpHelper.QuickFadeIn(canvas, Consts.GUI_IN_GAME_MENU_PAGE_FADE_TIME,
                LerpHelper.Type.SmoothStep);

            yield return StartCoroutine(OpenCategory(categories[0]));
        }

        public IEnumerator Close()
        {
            yield return StartCoroutine(openCategory.Save());
            yield return LerpHelper.QuickFadeOut(GetComponent<CanvasGroup>(), Consts.GUI_IN_GAME_MENU_PAGE_FADE_TIME,
                LerpHelper.Type.SmoothStep);
            openCategory = null;
            gameObject.SetActive(false);
        }

        public void Input(GameInputManager inputManager)
        {
            return;
        }

        public IEnumerator OpenCategory(SettingsCategory category)
        {
            if(category == openCategory)
                yield break;

            openingPage = true;

            if(openCategory != null)
            {
                yield return StartCoroutine(openCategory.Save());
                yield return StartCoroutine(openCategory.Close());
            }

            yield return StartCoroutine(category.Open());

            openCategory = category;
            openingPage = false;
        }

    }
}