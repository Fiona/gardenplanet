using System;
using System.Collections;
using UnityEditorInternal;
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
    public class AnimatedButton: MonoBehaviour
    {
        private EventTrigger eventTrigger;
        private Action callback;

        public void Awake()
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
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
        }

        public void SetCallback(Action callback)
        {
            this.callback = callback;
        }

        protected virtual void Hover(BaseEventData data)
        {
            StartCoroutine(HoverAnimation(data));
        }

        protected virtual void HoverEnd(BaseEventData data)
        {
            StartCoroutine(HoverEndAnimation(data));
        }

        protected virtual void Click(BaseEventData data)
        {
            StartCoroutine(ClickAnimation(data));
            if(callback == null)
                return;
            callback();
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

    }
}