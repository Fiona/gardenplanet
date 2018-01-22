using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StompyBlondie
{
    public class SelectableAnimatedButton: AnimatedButton
    {
        [Header("Selecteable Animated Button Settings")]
        public bool canDeselect;

        protected bool selected;

        public virtual void Select(BaseEventData data = null)
        {
            selected = true;
            StartCoroutine(SelectAnimation(data));
        }

        public virtual void Deselect(BaseEventData data = null)
        {
            selected = false;
            StartCoroutine(DeselectAnimation(data));
        }

        protected override void Hover(BaseEventData data)
        {
            if(selected)
                return;
            base.Hover(data);
        }

        protected override void HoverEnd(BaseEventData data)
        {
            if(selected)
                return;
            base.HoverEnd(data);
        }

        protected override void Click(BaseEventData data)
        {
            if(selected)
            {
                if(canDeselect)
                    Deselect(data);
                return;
            }
            base.Click(data);
            Select(data);
        }

        protected virtual IEnumerator SelectAnimation(BaseEventData data)
        {
            yield break;
        }

        protected virtual IEnumerator DeselectAnimation(BaseEventData data)
        {
            yield break;
        }

    }
}