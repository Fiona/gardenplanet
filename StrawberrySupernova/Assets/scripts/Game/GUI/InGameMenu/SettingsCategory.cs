using System.Collections;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using StompyBlondie;

namespace GardenPlanet
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SettingsCategory: MonoBehaviour
    {
        [Header("Common category references")]
        public SettingsCategoryButton button;

        private InGameMenuPageSettings settings;
        private CanvasGroup page;
        protected GUINavigator categoryButtonNavigator;
        protected bool isOpen = false;

        public virtual void Init(GUINavigator categoryButtonNavigator)
        {
            this.categoryButtonNavigator = categoryButtonNavigator;
            settings = FindObjectOfType<InGameMenuPageSettings>();
            page = GetComponent<CanvasGroup>();
            button.SetCallback(() =>
            {
                settings.StartCoroutine(settings.OpenCategory(this));
            });
            page.alpha = 0f;
            page.blocksRaycasts = false;
        }

        public virtual IEnumerator Open()
        {
            if(!button.selected)
                button.Select();
            isOpen = true;
            yield return LerpHelper.QuickFadeIn(page, .2f, LerpHelper.Type.SmoothStep);
            page.blocksRaycasts = true;
        }

        public virtual IEnumerator Close()
        {
            button.Deselect();
            isOpen = false;
            yield return LerpHelper.QuickFadeOut(page, .2f, LerpHelper.Type.SmoothStep);
            page.blocksRaycasts = false;
        }

        public virtual IEnumerator Save()
        {
            yield break;
        }

    }
}