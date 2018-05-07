using System;
using StompyBlondie;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GardenPlanet
{
    public class ItemHotbarNameplate: MonoBehaviour
    {
        public TextMeshProUGUI text;
        public UICopySize background;

        private CanvasGroup canvasGroup;
        private bool show;

        public void Show(string itemName, Transform hotbarItem)
        {
            if(!show)
            {
                StartCoroutine(LerpHelper.QuickFadeIn(canvasGroup, .25f, lerpType: LerpHelper.Type.SmoothStep));
                show = true;
            }
            text.text = itemName;
            text.ForceMeshUpdate();
            transform.position = new Vector3(
                hotbarItem.position.x,
                transform.position.y,
                transform.position.z
                );
        }

        public void Hide()
        {
            if(show)
                StartCoroutine(LerpHelper.QuickFadeOut(canvasGroup, .25f, lerpType: LerpHelper.Type.SmoothStep));
            show = false;
        }

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            show = false;
        }

    }
}