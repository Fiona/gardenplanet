using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using LitJson;
using Debug = UnityEngine.Debug;

namespace StrawberryNova
{
    public class AppearenceFieldColour: AppearenceField
    {
        [Header("Object references")]
        public Slider slider;
        public GlobalButton paletteButton;

        private Color currentVal;
        private List<Color> values;

        private new void Awake()
        {
            base.Awake();
            values = new List<Color>();
        }

        private new void Start()
        {
            controller = FindObjectOfType<CreateACharacterController>();

            var typeName = overrideConfigName == "" ? typeShortName : overrideConfigName;

            try
            {
                var items = controller.globalConfig["appearence"][typeName];
                foreach(JsonData item in items)
                {
                    if(item.Count < 3)
                        continue;
                    var h = (int)item[0] / 255f;
                    var s = (int)item[1] / 255f;
                    var v = (int)item[2] / 255f;
                    values.Add(Color.HSVToRGB(h, s, v));
                }
            }
            catch(KeyNotFoundException)
            {
                Debug.LogWarningFormat("Can't find definitions for appearence type {0}", typeName);
            }

            if(values.Count == 0)
                currentVal = Color.white;
            else
                currentVal = values[0];

            slider.wholeNumbers = true;
            slider.minValue = 0f;
            slider.maxValue = values.Count-1;
            slider.onValueChanged.AddListener(OnDragSlider);

            paletteButton.SetCallback(PaletteButtonPressed);
            base.Start();
        }

        private void PaletteButtonPressed()
        {
            CACColourPicker.ShowColourPicker(paletteButton, currentVal, ColourPickerCallback);
        }

        private void ColourPickerCallback(Color newColour)
        {
            currentVal = newColour;
            UpdateDisplayAndCharacter();
        }

        private void OnDragSlider(float val)
        {
            currentVal = values[(int)val];
            UpdateDisplayAndCharacter();
        }

        protected override void UpdateDisplayAndCharacter()
        {
            controller.character.SetAppearenceValue(typeShortName, currentVal);
        }

    }
}