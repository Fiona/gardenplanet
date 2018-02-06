using StompyBlondie;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace StrawberryNova
{
    public class SettingsCategoryButton: SelectableAnimatedButton
    {
        [Header("Animation Settings")]
        public float clickAnimTime = .2f;
        public float hoverFadeTime = .25f;

        [Header("Object references")]
        public Image backgroundNormal;
        public Image backgroundHover;
        public Image backgroundPressed;

        private InGameMenuPageSettings settings;

        public new void Start()
        {
            base.Start();
            settings = FindObjectOfType<InGameMenuPageSettings>();
            backgroundHover.color = new Color(1f, 1f, 1f, 0f);
            backgroundPressed.color = new Color(1f, 1f, 1f, 0f);
        }

        protected override void Click(BaseEventData data)
        {
            if(settings.openingPage)
                return;
            base.Click(data);
        }

        protected override IEnumerator HoverAnimation(BaseEventData data)
        {
            yield return LerpHelper.QuickFadeIn(backgroundHover, hoverFadeTime, LerpHelper.Type.SmoothStep);
        }

        protected override IEnumerator HoverEndAnimation(BaseEventData data)
        {
            yield return LerpHelper.QuickFadeOut(backgroundHover, hoverFadeTime, LerpHelper.Type.SmoothStep);
        }

        protected override IEnumerator SelectAnimation(BaseEventData data)
        {
            StartCoroutine(LerpHelper.QuickFadeOut(backgroundNormal, clickAnimTime, LerpHelper.Type.SmoothStep));
            StartCoroutine(LerpHelper.QuickFadeOut(backgroundHover, clickAnimTime, LerpHelper.Type.SmoothStep));
            StartCoroutine(
                LerpHelper.QuickTween(
                    (v) => gameObject.transform.localScale = v,
                    Vector3.one, new Vector3(.95f, .95f, .95f),
                    clickAnimTime,
                    lerpType:LerpHelper.Type.SmoothStep
                )
            );
            yield return LerpHelper.QuickFadeIn(backgroundPressed, clickAnimTime, LerpHelper.Type.SmoothStep);
        }

        protected override IEnumerator DeselectAnimation(BaseEventData data)
        {
            StartCoroutine(LerpHelper.QuickFadeIn(backgroundNormal, clickAnimTime, LerpHelper.Type.SmoothStep));
            StartCoroutine(
                LerpHelper.QuickTween(
                    (v) => gameObject.transform.localScale = v,
                    new Vector3(.9f, .9f, .9f), Vector3.one,
                    clickAnimTime,
                    lerpType:LerpHelper.Type.BounceOut
                )
            );
            yield return LerpHelper.QuickFadeOut(backgroundPressed, clickAnimTime, LerpHelper.Type.SmoothStep);
        }

    }
}