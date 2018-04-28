using System;
using System.Collections;
using StrawberryNova;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StompyBlondie
{
    /*
     * Subclassable button for adding more sophisticated code-driven animations.
     * After creating SetCallback should be called to pass it the function to be run on-click.
     *
     * Hover, HoverEnd and Click are virtual event callbacks that are called when those events happen.
     * They each have an equivalent virtual animation functions that are coroutines which are started by the previous
     * event callbacks - HoverAnimation, HoverEndAnimation, ClickAnimation.
     */
    [RequireComponent(typeof(RectTransform))]
    public class AnimatedButton: MonoBehaviour
    {
        public RectTransform rectTransform;
        public bool repeatable = true;

        private EventTrigger eventTrigger;
        private Action callback;
        private GameInputManager inputManager;
        private bool hover;
        private bool hasClicked = false;

        public void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            eventTrigger = gameObject.AddComponent<EventTrigger>();
            inputManager = FindObjectOfType<GameInputManager>();
        }

        public void Start()
        {
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

            var pointerDownEntry = new EventTrigger.Entry();
            pointerDownEntry.eventID = EventTriggerType.PointerDown;
            pointerDownEntry.callback.AddListener(PointerDown);
            eventTrigger.triggers.Add(pointerDownEntry);

            var pointerUpEntry = new EventTrigger.Entry();
            pointerUpEntry.eventID = EventTriggerType.PointerUp;
            pointerUpEntry.callback.AddListener(PointerUp);
            eventTrigger.triggers.Add(pointerUpEntry);
        }

        public void Update()
        {
            if(hover)
                inputManager.SetMouseTexture(inputManager.mouseHover, true);
        }

        public void SetCallback(Action callback)
        {
            this.callback = callback;
        }

        protected virtual void Hover(BaseEventData data)
        {
            hover = true;
            if(!repeatable && hasClicked)
                return;
            StartCoroutine(HoverAnimation(data));
        }

        protected virtual void HoverEnd(BaseEventData data)
        {
            hover = false;
            if(!repeatable && hasClicked)
                return;
            StartCoroutine(HoverEndAnimation(data));
        }

        protected virtual void PointerDown(BaseEventData data)
        {
            if(!repeatable && hasClicked)
                return;
            StartCoroutine(PointerDownAnimation(data));
        }

        protected virtual void PointerUp(BaseEventData data)
        {
            if(!repeatable && hasClicked)
                return;
            StartCoroutine(PointerUpAnimation(data));
        }

        protected virtual void Click(BaseEventData data)
        {
            if(!repeatable && hasClicked)
                return;
            hasClicked = true;
            StartCoroutine(DoClick(data));
        }

        protected virtual IEnumerator DoClick(BaseEventData data)
        {
            if(callback != null)
                callback();
            yield return ClickAnimation(data);
        }


        protected virtual IEnumerator HoverAnimation(BaseEventData data)
        {
            yield break;
        }

        protected virtual IEnumerator HoverEndAnimation(BaseEventData data)
        {
            yield break;
        }

        protected virtual IEnumerator ClickAnimation(BaseEventData data)
        {
            yield break;
        }

        protected virtual IEnumerator PointerDownAnimation(BaseEventData data)
        {
            yield break;
        }

        protected virtual IEnumerator PointerUpAnimation(BaseEventData data)
        {
            yield break;
        }

    }
}