using UnityEngine.UI;
using UnityEngine;

namespace StrawberryNova
{
    public class AppearenceFieldOpacity : AppearenceField
    {
        public string flipHorizontalParameterName;

        [Header("Object references")]
        public Slider slider;
        public GlobalButton flipButton;

        private float currentVal;

        private new void Awake()
        {
            base.Awake();
            slider.minValue = .25f;
            slider.maxValue = 1f;
            slider.onValueChanged.AddListener(OnDragSlider);

            flipButton.SetCallback(() => controller.character.ToggleAppearenceValue(flipHorizontalParameterName));
        }

        private new void Start()
        {
            controller = FindObjectOfType<CreateACharacterController>();
            SetValue(1f);
            base.Start();
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

        protected override void UpdateDisplayAndCharacter()
        {
            controller.character.SetAppearenceValue(typeShortName, currentVal);
        }

    }
}