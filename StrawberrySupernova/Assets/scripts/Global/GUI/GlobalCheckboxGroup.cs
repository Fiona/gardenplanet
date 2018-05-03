using System.Collections.Generic;
using UnityEngine;

namespace GardenPlanet
{
    public class GlobalCheckboxGroup: MonoBehaviour
    {
        public bool allowSwitchOff;
        public bool hasGUINavigator = false;
        public GlobalCheckbox[] checkboxes;

        [HideInInspector]
        public GUINavigator navigator;

        private int value;
        private GlobalCheckbox currentCheckbox;

        public void SelectedCheckbox(GlobalCheckbox checkbox)
        {
            value = checkbox.value;
            if(currentCheckbox != null)
                currentCheckbox.Deselect();
            currentCheckbox = checkbox;
        }

        public void SetValue(int _value)
        {
            value = _value;
            if(currentCheckbox != null)
                currentCheckbox.Deselect();
            foreach(var checkbox in checkboxes)
                if(checkbox.value == value)
                    checkbox.Select();
        }

        public int GetValue()
        {
            return value;
        }

        private void Awake()
        {
            if(hasGUINavigator)
            {
                navigator = gameObject.AddComponent<GUINavigator>();
                navigator.direction = GUINavigator.GUINavigatorDirection.Horizontal;
                navigator.oppositeAxisLinking = true;
                navigator.allowWrapping = true;
                navigator.active = false;
            }
            foreach(var checkbox in checkboxes)
                RegisterCheckbox(checkbox);
        }

        private void RegisterCheckbox(GlobalCheckbox checkbox)
        {
            if(hasGUINavigator)
                navigator.AddNavigationElement(checkbox.rectTransform);
            checkbox.checkboxGroup = this;
            checkbox.canDeselect = allowSwitchOff;
        }

    }
}