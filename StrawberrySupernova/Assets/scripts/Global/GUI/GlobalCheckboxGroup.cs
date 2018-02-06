using System.Collections.Generic;
using UnityEngine;

namespace StrawberryNova
{
    public class GlobalCheckboxGroup: MonoBehaviour
    {
        public bool allowSwitchOff;

        private int value;
        private GlobalCheckbox currentCheckbox;
        private List<GlobalCheckbox> checkboxes;

        private void Awake()
        {
            checkboxes = new List<GlobalCheckbox>();
        }

        public void RegisterCheckbox(GlobalCheckbox checkbox)
        {
            checkboxes.Add(checkbox);
        }

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
    }
}