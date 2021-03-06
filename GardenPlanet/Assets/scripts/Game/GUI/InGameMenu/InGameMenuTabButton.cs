using System.Collections;
using System;
using StompyBlondie;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;
using StompyBlondie.Utils;

namespace GardenPlanet
{
    public class InGameMenuTabButton: SelectableAnimatedButton
    {
        [Header("Object references")]
        public Image backgroundNormal;
        public Image backgroundHover;
        public Image backgroundSelected;
        public Image icon;
        public RectTransform display;

        [Header("Settings")]
        public float hiddenDisplayX;
        public float shownDisplayX;
        public float selectedDisplayX;

        private IEnumerator glowAnim;

        public void Initialise(string id, string displayName, Action callback)
        {
            // Hide certain images
            backgroundHover.color = new Color(backgroundHover.color.r, backgroundHover.color.g, backgroundHover.color.b, 0f);
            backgroundSelected.color = new Color(backgroundHover.color.r, backgroundHover.color.g, backgroundHover.color.b, 0f);

            // Set text
            gameObject.SetActive(true);
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = displayName;

            // Set icon
            icon.sprite = Resources.Load<Sprite>(Consts.IN_GAME_MENU_TAB_ICONS_PATH_PREFIX + id);
            icon.SetNativeSize();

            // Do slide in
            StartCoroutine(
                LerpHelper.QuickTween(
                    (v) => { display.anchoredPosition = v; },
                    new Vector2(hiddenDisplayX, 0f),
                    new Vector2(shownDisplayX, 0f),
                    .2f, lerpType:LerpHelper.Type.SmoothStep
                )
            );

            SetCallback(callback);
            canDeselect = false;
        }

        protected override IEnumerator HoverAnimation(BaseEventData data)
        {
            yield return LerpHelper.QuickFadeIn(backgroundHover, .25f, LerpHelper.Type.SmoothStep);
        }

        protected override IEnumerator HoverEndAnimation(BaseEventData data)
        {
            yield return LerpHelper.QuickFadeOut(backgroundHover, .25f, LerpHelper.Type.SmoothStep);
        }

        protected override IEnumerator SelectAnimation(BaseEventData data)
        {
            StartCoroutine(LerpHelper.QuickFadeOut(backgroundHover, .2f, LerpHelper.Type.SmoothStep));
            StartCoroutine(LerpHelper.QuickFadeIn(backgroundSelected, .2f, LerpHelper.Type.SmoothStep));
            yield return LerpHelper.QuickTween(
                (v) => { display.anchoredPosition = v; },
                new Vector2(shownDisplayX, 0f),
                new Vector2(selectedDisplayX, 0f),
                1f, lerpType:LerpHelper.Type.BounceOut
            );
            if(glowAnim == null)
            {
                glowAnim = GlowAnim();
                StartCoroutine(glowAnim);
            }
        }

        protected override IEnumerator DeselectAnimation(BaseEventData data)
        {
            if(glowAnim != null)
                StopCoroutine(glowAnim);
            glowAnim = null;
            StartCoroutine(LerpHelper.QuickTween(BackgroundSelectedFade, backgroundSelected.color.a,
                0f, .2f, lerpType:LerpHelper.Type.SmoothStep));
            yield return LerpHelper.QuickTween(
                (v) => { display.anchoredPosition = v; },
                new Vector2(selectedDisplayX, 0f),
                new Vector2(shownDisplayX, 0f),
                1f, lerpType: LerpHelper.Type.BounceOut
            );
        }

        public void SlideOut()
        {
            StartCoroutine(
                LerpHelper.QuickTween(
                    (v) => { display.anchoredPosition = v; },
                    new Vector2(selected ? selectedDisplayX : shownDisplayX, 0f),
                    new Vector2(hiddenDisplayX, 0f),
                    .2f, lerpType:LerpHelper.Type.SmoothStep
                )
            );
        }

        private IEnumerator GlowAnim()
        {
            while(true)
            {
                if(!selected)
                    yield break;
                yield return LerpHelper.QuickTween(
                    BackgroundSelectedFade, 1f, .5f, 2f, lerpType:LerpHelper.Type.SmoothStep
                );
                yield return LerpHelper.QuickTween(
                    BackgroundSelectedFade, .5f, 1f, 2f, lerpType:LerpHelper.Type.SmoothStep
                );
            }
        }

        private void BackgroundSelectedFade(float v)
        {
            backgroundSelected.color = new Color(
                backgroundSelected.color.r, backgroundSelected.color.g, backgroundSelected.color.b, v
            );
        }

    }
}