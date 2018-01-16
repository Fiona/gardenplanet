using System;
using System.Collections;
using System.Linq;
using StompyBlondie;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace StrawberryNova
{
    public class InGameMenuTabButton: MonoBehaviour
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

        private Action callback;
        private bool selected;

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

            // Set up events
            this.callback = callback;
            var eventTrigger = GetComponent<EventTrigger>();

            var hoverEntry = new EventTrigger.Entry();
            hoverEntry.eventID = EventTriggerType.PointerEnter;
            hoverEntry.callback.AddListener(Hover);
            eventTrigger.triggers.Add(hoverEntry);

            var exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener(HoverEnd);
            eventTrigger.triggers.Add(exitEntry);

            var clickEntry = new EventTrigger.Entry();
            clickEntry.eventID = EventTriggerType.PointerClick;
            clickEntry.callback.AddListener(Click);
            eventTrigger.triggers.Add(clickEntry);

            // Do slide in
            StartCoroutine(
                LerpHelper.QuickTween(
                    (v) => { display.anchoredPosition = v; },
                    new Vector2(hiddenDisplayX, 0f),
                    new Vector2(shownDisplayX, 0f),
                    .2f, lerpType:LerpHelper.Type.SmoothStep
                )
            );
        }

        public void Hover(BaseEventData data)
        {
            if(selected)
                return;
            StartCoroutine(LerpHelper.QuickFadeIn(backgroundHover, .25f, LerpHelper.Type.SmoothStep));
        }

        public void HoverEnd(BaseEventData data)
        {
            if(selected)
                return;
            StartCoroutine(LerpHelper.QuickFadeOut(backgroundHover, .25f, LerpHelper.Type.SmoothStep));
        }

        public void Click(BaseEventData data)
        {
            if(selected)
                return;
            callback();
        }

        public void Select()
        {
            if(selected)
                return;
            StartCoroutine(LerpHelper.QuickFadeOut(backgroundHover, .2f, LerpHelper.Type.SmoothStep));
            StartCoroutine(LerpHelper.QuickFadeIn(backgroundSelected, .2f, LerpHelper.Type.SmoothStep));
            StartCoroutine(
                LerpHelper.QuickTween(
                    (v) => { display.anchoredPosition = v; },
                    new Vector2(shownDisplayX, 0f),
                    new Vector2(selectedDisplayX, 0f),
                    1f, lerpType:LerpHelper.Type.BounceOut
                )
            );
            selected = true;
        }

        public void Deselect()
        {
            if(!selected)
                return;
            StartCoroutine(LerpHelper.QuickFadeOut(backgroundSelected, .2f, LerpHelper.Type.SmoothStep));
            StartCoroutine(
                LerpHelper.QuickTween(
                    (v) => { display.anchoredPosition = v; },
                    new Vector2(selectedDisplayX, 0f),
                    new Vector2(shownDisplayX, 0f),
                    1f, lerpType:LerpHelper.Type.BounceOut
                )
            );
            selected = false;
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

    }
}