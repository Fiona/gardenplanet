using System;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace GardenPlanet
{
    public class AppearenceFieldOpacity : AppearenceField
    {
        public string flipHorizontalParameterName;
        public float inputChangeValueSpeed = .01f;

        [Header("Object references")]
        public Slider slider;
        public GlobalButton flipButton;

        [HideInInspector]
        public GUINavigator navigator;

        private GameInputManager input;
        private float currentVal;
        private bool adjusting;
        private int adjustingBuffer;

        public override void Randomise()
        {
            currentVal = Random.Range(slider.minValue, slider.maxValue);
            slider.value = currentVal;
            if(Random.value < .5f)
                controller.character.ToggleAppearenceValue(flipHorizontalParameterName);
            UpdateDisplayAndCharacter();
        }

        private new void Awake()
        {
            base.Awake();
            input = FindObjectOfType<GameInputManager>();

            slider.minValue = .25f;
            slider.maxValue = 1f;
            slider.onValueChanged.AddListener(OnDragSlider);

            // Add pointer events to slider for non-mouse inputs
            var eventTrigger = slider.gameObject.AddComponent<EventTrigger>();
            var pointerUpEntry = new EventTrigger.Entry();
            pointerUpEntry.eventID = EventTriggerType.PointerUp;
            pointerUpEntry.callback.AddListener(PointerUp);
            eventTrigger.triggers.Add(pointerUpEntry);

            flipButton.SetCallback(() => controller.character.ToggleAppearenceValue(flipHorizontalParameterName));
        }

        private new void Start()
        {
            controller = FindObjectOfType<CreateACharacterController>();
            SetValue(1f);
            base.Start();
        }

        private void Update()
        {
            slider.interactable = input.mouseMode;

            // Non-mouse mode inputs
            if(adjusting)
            {
                adjustingBuffer++;
                if(input.player.GetButton("Menu Left"))
                    slider.value -= inputChangeValueSpeed;
                if(input.player.GetButton("Menu Right"))
                    slider.value += inputChangeValueSpeed;
                if(adjustingBuffer > 1 &&
                   (input.player.GetButtonUp("Confirm") || input.player.GetButtonUp("Cancel") || input.mouseMode))
                {
                    adjusting = false;
                    navigator.active = true;
                }
            }
        }

        private void OnDragSlider(float val)
        {
            SetValue(val);
        }

        private void SetValue(float val)
        {
            currentVal = val;
            slider.value = val;
            UpdateDisplayAndCharacter();
        }

        private void PointerUp(BaseEventData data)
        {
            if(!input.mouseMode && !adjusting)
            {
                adjusting = true;
                navigator.active = false;
                adjustingBuffer = 0;
            }
        }

        protected override void UpdateDisplayAndCharacter()
        {
            controller.character.SetAppearenceValue(typeShortName, currentVal);
        }

    }
}