using System.Collections;
using StompyBlondie;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StrawberryNova
{
    public class GlobalButton: AnimatedButton
    {
        [Header("Object references")]
        public Image backgroundNormal;
        public Image backgroundHover;
        public Image backgroundPressed;

        private float clickAnimTime = .1f;
        private float hoverFadeTime = .15f;

        public new void Start()
        {
            base.Start();
            backgroundHover.color = new Color(1f, 1f, 1f, 0f);
            backgroundPressed.color = new Color(1f, 1f, 1f, 0f);
        }

        protected override IEnumerator HoverAnimation(BaseEventData data)
        {
            yield return LerpHelper.QuickFadeIn(backgroundHover, hoverFadeTime, LerpHelper.Type.SmoothStep);
        }

        protected override IEnumerator HoverEndAnimation(BaseEventData data)
        {
            yield return LerpHelper.QuickFadeOut(backgroundHover, hoverFadeTime, LerpHelper.Type.SmoothStep);
        }

        protected override IEnumerator PointerDownAnimation(BaseEventData data)
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

        protected override IEnumerator PointerUpAnimation(BaseEventData data)
        {
            StartCoroutine(LerpHelper.QuickFadeIn(backgroundNormal, clickAnimTime, LerpHelper.Type.SmoothStep));
            StartCoroutine(
                LerpHelper.QuickTween(
                    (v) => gameObject.transform.localScale = v,
                    new Vector3(.9f, .9f, .9f), Vector3.one,
                    clickAnimTime/2f,
                    lerpType:LerpHelper.Type.BounceOut
                )
            );
            yield return LerpHelper.QuickFadeOut(backgroundPressed, clickAnimTime, LerpHelper.Type.SmoothStep);
        }

        protected override IEnumerator ClickAnimation(BaseEventData data)
        {
            StartCoroutine(LerpHelper.QuickFadeIn(backgroundNormal, clickAnimTime, LerpHelper.Type.SmoothStep));
            StartCoroutine(LerpHelper.QuickFadeOut(backgroundPressed, clickAnimTime, LerpHelper.Type.SmoothStep));
            yield return LerpHelper.QuickTween(
                (v) => gameObject.transform.localScale = v,
                new Vector3(.9f, .9f, .9f), Vector3.one,
                clickAnimTime * 2,
                lerpType: LerpHelper.Type.BounceOut
            );
        }

    }
}