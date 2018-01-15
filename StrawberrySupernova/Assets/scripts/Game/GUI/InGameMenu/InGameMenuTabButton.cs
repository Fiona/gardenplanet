using System;
using System.Collections;
using System.Linq;
using StompyBlondie;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.UIElements;
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

        private Action callback;

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
        }

        public void Hover(BaseEventData data)
        {
            StartCoroutine(LerpHelper.QuickFadeIn(backgroundHover, .25f));
        }

        public void HoverEnd(BaseEventData data)
        {
            StartCoroutine(LerpHelper.QuickFadeOut(backgroundHover, .25f));
        }

        public void Click(BaseEventData data)
        {
            callback();
        }

    }
}