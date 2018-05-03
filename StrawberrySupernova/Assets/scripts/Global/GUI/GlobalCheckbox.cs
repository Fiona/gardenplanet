using StompyBlondie;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace GardenPlanet
{
    public class GlobalCheckbox: SelectableAnimatedButton
    {
        [Header("Settings")]
        public int value;

        [Header("Animation Settings")]
        public float clickAnimTime = .2f;
        public float hoverFadeTime = .25f;

        [Header("Object references")]
        public Image backgroundNormal;
        public Image backgroundHover;
        public Image backgroundPressed;

        [HideInInspector]
        public GlobalCheckboxGroup checkboxGroup;

        public new void Start()
        {
            base.Start();
            backgroundHover.color = new Color(1f, 1f, 1f, 0f);
            backgroundPressed.color = new Color(1f, 1f, 1f, 0f);
        }

        public override void Select(BaseEventData data = null)
        {
            if(checkboxGroup != null)
                checkboxGroup.SelectedCheckbox(this);
            base.Select(data);
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
            yield return LerpHelper.QuickFadeIn(backgroundPressed, clickAnimTime, LerpHelper.Type.SmoothStep);
        }

        protected override IEnumerator DeselectAnimation(BaseEventData data)
        {
            StartCoroutine(LerpHelper.QuickFadeIn(backgroundNormal, clickAnimTime, LerpHelper.Type.SmoothStep));
            yield return LerpHelper.QuickFadeOut(backgroundPressed, clickAnimTime, LerpHelper.Type.SmoothStep);
        }

    }
}