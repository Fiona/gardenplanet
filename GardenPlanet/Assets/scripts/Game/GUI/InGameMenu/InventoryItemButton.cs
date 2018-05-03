using System.Collections;
using StompyBlondie;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GardenPlanet
{
    public class InventoryItemButton: SelectableAnimatedButton
    {
        [Header("Inventory Item Settings")]
        public float deselectedScaleSize = .98f;
        public float pressedScaleSize = .96f;
        public float selectAnimTime = .2f;
        public float hoverFadeTime = .25f;

        [Header("Object references")]
        public TextMeshProUGUI itemName;
        public TextMeshProUGUI quantity;
        public Image image;
        public Image backgroundNormal;
        public Image backgroundHover;
        public Image backgroundSelected;

        private IEnumerator glowAnim;

        public void Initialise(Inventory.InventoryItemEntry item)
        {
            itemName.text = item.itemType.DisplayName;
            quantity.text = item.quantity.ToString();
            image.sprite = item.itemType.Image;

            transform.localScale = new Vector3(deselectedScaleSize, deselectedScaleSize, deselectedScaleSize);
            backgroundHover.color = new Color(1f, 1f, 1f, 0f);
            backgroundSelected.color = new Color(1f, 1f, 1f, 0f);
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
            if(selected)
                yield break;
            yield return StartCoroutine(
                LerpHelper.QuickTween(
                    (v) => gameObject.transform.localScale = v,
                    new Vector3(deselectedScaleSize, deselectedScaleSize, deselectedScaleSize),
                    new Vector3(pressedScaleSize, pressedScaleSize, pressedScaleSize),
                    selectAnimTime,
                    lerpType:LerpHelper.Type.SmoothStep
                )
            );
        }

        protected override IEnumerator PointerUpAnimation(BaseEventData data)
        {
            if(selected)
                yield break;
            yield return StartCoroutine(
                LerpHelper.QuickTween(
                    (v) => gameObject.transform.localScale = v,
                    new Vector3(pressedScaleSize, pressedScaleSize, pressedScaleSize),
                    new Vector3(deselectedScaleSize, deselectedScaleSize, deselectedScaleSize),
                    selectAnimTime,
                    lerpType:LerpHelper.Type.SmoothStep
                )
            );
        }

        protected override IEnumerator SelectAnimation(BaseEventData data)
        {
            StartCoroutine(LerpHelper.QuickFadeOut(backgroundHover, selectAnimTime, LerpHelper.Type.SmoothStep));
            StartCoroutine(LerpHelper.QuickFadeIn(backgroundSelected, selectAnimTime, LerpHelper.Type.SmoothStep));
            yield return LerpHelper.QuickTween(
                (v) => { transform.localScale = v; },
                new Vector3(deselectedScaleSize, deselectedScaleSize, deselectedScaleSize),
                Vector3.one,
                selectAnimTime, lerpType: LerpHelper.Type.SmoothStep
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
                0f, selectAnimTime, lerpType:LerpHelper.Type.SmoothStep));
            yield return LerpHelper.QuickTween(
                (v) => { transform.localScale = v; },
                Vector3.one,
                new Vector3(deselectedScaleSize, deselectedScaleSize, deselectedScaleSize),
                selectAnimTime, lerpType:LerpHelper.Type.EaseIn
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