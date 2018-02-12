using System.Collections;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using StompyBlondie;

namespace StrawberryNova
{
    [RequireComponent(typeof(CanvasGroup))]
    public class SettingsCategory: MonoBehaviour
    {
        [Header("Common category references")]
        public SettingsCategoryButton button;

        private InGameMenuPageSettings settings;
        private CanvasGroup page;
        protected GUINavigator categoryButtonNavigator;

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
            yield return LerpHelper.QuickFadeIn(page, .2f, LerpHelper.Type.SmoothStep);
            page.blocksRaycasts = true;
        }

        public virtual IEnumerator Close()
        {
            button.Deselect();
            yield return LerpHelper.QuickFadeOut(page, .2f, LerpHelper.Type.SmoothStep);
            page.blocksRaycasts = false;
        }

        public virtual IEnumerator Save()
        {
            yield break;
        }

    }
}