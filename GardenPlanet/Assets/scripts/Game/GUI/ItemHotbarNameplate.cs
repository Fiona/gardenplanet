using System;
using StompyBlondie;
using TMPro;
using UnityEngine;

namespace GardenPlanet
{
    public class ItemHotbarNameplate: MonoBehaviour
    {
        public TextMeshProUGUI text;

        private CanvasGroup canvasGroup;

        public void Show(string itemName, Transform hotbarItem)
        {
            if(Math.Abs(canvasGroup.alpha) < .05f)
                StartCoroutine(LerpHelper.QuickFadeIn(canvasGroup, .5f, lerpType: LerpHelper.Type.SmoothStep));
            canvasGroup.alpha = 1f;
            text.text = itemName;
            transform.position = new Vector3(
                hotbarItem.position.x,
                transform.position.y,
                transform.position.z
                );
        }

        public void Hide()
        {
            if(Math.Abs(canvasGroup.alpha) > .95f)
                StartCoroutine(LerpHelper.QuickFadeOut(canvasGroup, .5f, lerpType: LerpHelper.Type.SmoothStep));
            canvasGroup.alpha = 0f;
        }

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }

    }
}