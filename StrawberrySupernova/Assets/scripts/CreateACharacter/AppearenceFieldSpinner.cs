using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace StrawberryNova
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
                var items = controller.globalConfig["appearence"][typeName];
                foreach(var item in items.Keys)
                    if((bool) items[item]["unlocked_at_start"])
                        values.Add(new string[] {item, (string) items[item]["name"]});
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