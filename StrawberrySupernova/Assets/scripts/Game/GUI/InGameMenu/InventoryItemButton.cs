using System.Collections;
using StompyBlondie;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace StrawberryNova
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
            StartCoroutine(LerpHelper.QuickFadeOut(backgroundNormal, selectAnimTime, LerpHelper.Type.SmoothStep));
            StartCoroutine(LerpHelper.QuickFadeOut(backgroundHover, selectAnimTime, LerpHelper.Type.SmoothStep));
            StartCoroutine(LerpHelper.QuickFadeIn(backgroundSelected, selectAnimTime, LerpHelper.Type.SmoothStep));
            yield return LerpHelper.QuickTween(
                (v) => { transform.localScale = v; },
                new Vector3(deselectedScaleSize, deselectedScaleSize, deselectedScaleSize),
                Vector3.one,
                selectAnimTime, lerpType:LerpHelper.Type.SmoothStep
            );
        }

        protected override IEnumerator DeselectAnimation(BaseEventData data)
        {
            StartCoroutine(LerpHelper.QuickFadeIn(backgroundNormal, selectAnimTime, LerpHelper.Type.SmoothStep));
            StartCoroutine(LerpHelper.QuickFadeOut(backgroundSelected, selectAnimTime, LerpHelper.Type.SmoothStep));
            yield return LerpHelper.QuickTween(
                (v) => { transform.localScale = v; },
                Vector3.one,
                new Vector3(deselectedScaleSize, deselectedScaleSize, deselectedScaleSize),
                selectAnimTime, lerpType:LerpHelper.Type.EaseIn
            );
        }

    }
}