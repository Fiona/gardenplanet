using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

namespace GardenPlanet
{
    public class AppearenceFieldColour: AppearenceField
    {
        [Header("Object references")]
        public GameObject swatchTemplate;
        public GameObject swatchHolder;
        public GlobalButton paletteButton;

        [HideInInspector]
        public GUINavigator navigator;

        private Color currentVal;
        private List<Color> values;
        private List<GlobalSelectableButton> swatches;

        public override void Randomise()
        {
            if(values.Count == 0)
                return;
            var num = Random.Range(0, values.Count);
            currentVal = values[num];
            DeselectAllSwatches();
            swatches[num].Select();
            UpdateDisplayAndCharacter();
        }

        private new void Awake()
        {
            base.Awake();
            values = new List<Color>();
            swatches = new List<GlobalSelectableButton>();
        }

        private new void Start()
        {
            controller = FindObjectOfType<CreateACharacterController>();

            var typeName = overrideConfigName == "" ? typeShortName : overrideConfigName;

            // Get colour values from global config
            try
            {
                var colourItems = new List<List<int>>();
                switch(typeName)
                {
                    case "skin_colours":
                        colourItems = controller.globalConfig.appearence.skinColours;
                        break;
                    case "eye_colours":
                        colourItems = controller.globalConfig.appearence.eyeColours;
                        break;
                    case "hair_colours":
                        colourItems = controller.globalConfig.appearence.hairColours;
                        break;
                    default:
                        throw new KeyNotFoundException();
                }
                foreach(var item in colourItems)
                {
                    if(item.Count < 3)
                        continue;
                    var h = item[0] / 255f;
                    var s = item[1] / 255f;
                    var v = item[2] / 255f;
                    values.Add(Color.HSVToRGB(h, s, v));
                }
            }
            catch(KeyNotFoundException)
            {
                Debug.LogWarningFormat("Can't find definitions for appearence type {0}", typeName);
            }

            // Iterate through colours and create swatches for them
            var index = 0;
            foreach(var value in values)
            {
                swatchTemplate.SetActive(false);
                var newSwatch = Instantiate(swatchTemplate) as GameObject;
                newSwatch.SetActive(true);
                newSwatch.transform.SetParent(swatchHolder.transform, false);
                newSwatch.GetComponent<Image>().color = value;
                var buttonComp = newSwatch.GetComponent<GlobalSelectableButton>();
                var copy = index;
                buttonComp.SetCallback(() => SwatchPressed(copy));
                swatches.Add(buttonComp);
                index++;
            }

            // Set fist colour as default
            if(values.Count == 0)
                currentVal = Color.white;
            else
                currentVal = values[0];

            paletteButton.SetCallback(PaletteButtonPressed);
            base.Start();
        }

        private void PaletteButtonPressed()
        {
            CACColourPicker.ShowColourPicker(paletteButton, currentVal, ColourPickerCallback, navigator);
        }

        private void ColourPickerCallback(Color newColour)
        {
            currentVal = newColour;
            DeselectAllSwatches();
            UpdateDisplayAndCharacter();
        }

        private void SwatchPressed(int index)
        {
            if(currentVal == values[index])
                return;
            currentVal = values[index];
            DeselectAllSwatches();
            swatches[index].Select();
            UpdateDisplayAndCharacter();
        }

        private void DeselectAllSwatches()
        {
            foreach(var swatch in swatches)
                if(swatch.selected)
                    swatch.Deselect();
        }

        protected override void UpdateDisplayAndCharacter()
        {
            controller.character.SetAppearenceValue(typeShortName, currentVal);
        }

    }
}