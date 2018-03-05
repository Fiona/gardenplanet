using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using TMPro;
using UnityEngine;

namespace StrawberryNova
{
    public class AppearenceField: MonoBehaviour
    {
        [Header("Settings")]
        public string typeShortName;
        public bool allowNone;
        public string overrideConfigName;

        [Header("Object references")]
        public GlobalButton previousButton;
        public GlobalButton nextButton;
        public TextMeshProUGUI displayText;

        private CreateACharacterController controller;
        private List<string[]> values;
        private int currentVal;

        private void Awake()
        {
            values = new List<string[]>();
        }

        private void Start()
        {
            controller = FindObjectOfType<CreateACharacterController>();
            previousButton.SetCallback(PreviousButtonPressed);
            nextButton.SetCallback(NextButtonPressed);

            if(allowNone)
                values.Add(new string[]{"", "None"});

            var typeName = overrideConfigName == "" ? typeShortName : overrideConfigName;
            var items = controller.globalConfig["appearence"][typeName];
            foreach(var item in items.Keys)
                if((bool)items[item]["unlocked_at_start"])
                    values.Add(new string[]{item, (string)items[item]["name"]});

            UpdateDisplayAndCharacter();
        }

        private void UpdateDisplayAndCharacter()
        {
            controller.character.SetAppearenceValue(typeShortName, values[currentVal][0]);
            displayText.text = values[currentVal][1];
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

    }
}