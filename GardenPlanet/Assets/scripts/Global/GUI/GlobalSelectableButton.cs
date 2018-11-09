using StompyBlondie;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;
using StompyBlondie.Utils;

namespace GardenPlanet
{
    public class GlobalSelectableButton: SelectableAnimatedButton
    {
        [Header("Animation Settings")]
        public float clickAnimTime = .2f;
        public float hoverFadeTime = .25f;
        public float scaleToOnSelect = .95f;

        [Header("Object references")]
        public Image backgroundNormal;
        public Image backgroundHover;
        public Image backgroundPressed;

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

        protected override IEnumerator SelectAnimation(BaseEventData data)
        {
            StartCoroutine(LerpHelper.QuickFadeOut(backgroundNormal, clickAnimTime, LerpHelper.Type.SmoothStep));
            StartCoroutine(LerpHelper.QuickFadeOut(backgroundHover, clickAnimTime, LerpHelper.Type.SmoothStep));
            StartCoroutine(
                LerpHelper.QuickTween(
                    (v) => gameObject.transform.localScale = v,
                    Vector3.one, new Vector3(scaleToOnSelect, scaleToOnSelect, scaleToOnSelect),
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
                    new Vector3(scaleToOnSelect, scaleToOnSelect, scaleToOnSelect), Vector3.one,
                    clickAnimTime,
                    lerpType:LerpHelper.Type.BounceOut
                )
            );
            yield return LerpHelper.QuickFadeOut(backgroundPressed, clickAnimTime, LerpHelper.Type.SmoothStep);
        }

    }
}