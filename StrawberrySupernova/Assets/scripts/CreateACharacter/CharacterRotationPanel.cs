using Rewired;
using UnityEngine;
using UnityEngine.EventSystems;

namespace StrawberryNova
{
    public class CharacterRotationPanel: MonoBehaviour
    {
        private CACCharacter character;
        private EventTrigger eventTrigger;
        private bool rotating;
        private bool hover;
        private GameInputManager inputManager;

        private void Start()
        {
            character = FindObjectOfType<CACCharacter>();
            inputManager = FindObjectOfType<GameInputManager>();

            eventTrigger = gameObject.AddComponent<EventTrigger>();

            var pointerDownEntry = new EventTrigger.Entry();
            pointerDownEntry.eventID = EventTriggerType.PointerDown;
            pointerDownEntry.callback.AddListener(PointerDown);
            eventTrigger.triggers.Add(pointerDownEntry);

            var pointerUpEntry = new EventTrigger.Entry();
            pointerUpEntry.eventID = EventTriggerType.PointerUp;
            pointerUpEntry.callback.AddListener(PointerUp);
            eventTrigger.triggers.Add(pointerUpEntry);

            var pointerHoverEntry = new EventTrigger.Entry();
            pointerHoverEntry.eventID = EventTriggerType.PointerEnter;
            pointerHoverEntry.callback.AddListener(PointerHoverEntry);
            eventTrigger.triggers.Add(pointerHoverEntry);

            var pointerHoverEnd = new EventTrigger.Entry();
            pointerHoverEnd.eventID = EventTriggerType.PointerExit;
            pointerHoverEnd.callback.AddListener(PointerHoverEnd);
            eventTrigger.triggers.Add(pointerHoverEnd);
        }

        private void FixedUpdate()
        {
            if(rotating)
            {
                var amount = ReInput.controllers.Mouse.screenPositionDelta.x;
                if(Mathf.Abs(amount) > 0f)
                    character.GrabRotate(amount * 2);
                inputManager.SetMouseTexture(inputManager.mouseOkay, true);
            }
            else
            {
                if(hover)
                    inputManager.SetMouseTexture(inputManager.mouseHover, true);
            }
        }

        private void PointerDown(BaseEventData data)
        {
            rotating = true;
        }

        private void PointerUp(BaseEventData data)
        {
            rotating = false;
        }

        private void PointerHoverEntry(BaseEventData data)
        {
            hover = true;
        }

        private void PointerHoverEnd(BaseEventData data)
        {
            hover = false;
        }

    }
}