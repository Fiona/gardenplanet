using System.Collections.Generic;
using TMPro;
using UnityEngine;
using AppearenceObject = GardenPlanet.GlobalConfig.AppearenceConfig.AppearenceObject;

namespace GardenPlanet
{
    public class AppearenceFieldSpinner: AppearenceField
    {
        public bool allowNone;

        [Header("Object references")]
        public GlobalButton previousButton;
        public GlobalButton nextButton;
        public TextMeshProUGUI displayText;

        private List<string[]> values;
        private int currentVal;

        public override void Randomise()
        {
            if(values.Count == 0)
                return;
            currentVal = Random.Range(0, values.Count);
            UpdateDisplayAndCharacter();
        }

        private new void Awake()
        {
            base.Awake();
            values = new List<string[]>();
        }

        private new void Start()
        {
            controller = FindObjectOfType<CreateACharacterController>();

            previousButton.SetCallback(PreviousButtonPressed);
            nextButton.SetCallback(NextButtonPressed);

            if(allowNone)
                values.Add(new string[]{"", "None"});

            var typeName = overrideConfigName == "" ? typeShortName : overrideConfigName;

            try
            {
                var appearenceObjects = new Dictionary<string, AppearenceObject>();
                switch(typeName)
                {
                    case "eyes":
                        appearenceObjects = controller.globalConfig.appearence.eyes;
                        break;
                    case "hair":
                        appearenceObjects = controller.globalConfig.appearence.hair;
                        break;
                    case "tops":
                        appearenceObjects = controller.globalConfig.appearence.tops;
                        break;
                    case "bottoms":
                        appearenceObjects = controller.globalConfig.appearence.bottoms;
                        break;
                    case "shoes":
                        appearenceObjects = controller.globalConfig.appearence.shoes;
                        break;
                    case "eyebrows":
                        appearenceObjects = controller.globalConfig.appearence.eyebrows;
                        break;
                    case "noses":
                        appearenceObjects = controller.globalConfig.appearence.noses;
                        break;
                    case "mouths":
                        appearenceObjects = controller.globalConfig.appearence.mouths;
                        break;
                    case "face_detail":
                        appearenceObjects = controller.globalConfig.appearence.faceDetail;
                        break;
                    case "head_accessories":
                        appearenceObjects = controller.globalConfig.appearence.headAccessories;
                        break;
                    default:
                        throw new KeyNotFoundException();
                }
                foreach(var item in appearenceObjects.Keys)
                    if(appearenceObjects[item].unlockedAtStart)
                        values.Add(new []{item, appearenceObjects[item].name});
            }
            catch(KeyNotFoundException)
            {
                Debug.LogWarningFormat("Can't find definitions for appearence type {0}", typeName);
            }

            base.Start();
        }

        private void PreviousButtonPressed()
        {
            currentVal--;
            if(currentVal < 0)
                currentVal = values.Count-1;
            UpdateDisplayAndCharacter();
        }

        private void NextButtonPressed()
        {
            currentVal++;
            if(currentVal >= values.Count)
                currentVal = 0;
            UpdateDisplayAndCharacter();
        }

        protected override void UpdateDisplayAndCharacter()
        {
            if(values.Count == 0)
                return;
            controller.character.SetAppearenceValue(typeShortName, values[currentVal][0]);
            displayText.text = values[currentVal][1];
        }

    }
}