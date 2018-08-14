using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace GardenPlanet
{
    public class AttributeField: MonoBehaviour
    {
        [Header("Common inputs")]
        public InputField keyInput;
        public Dropdown typeInput;
        public Button deleteButton;

        [Header("Boolean value")]
        public GameObject booleanHolder;
        public Toggle booleanTrue;
        public Toggle booleanFalse;

        [Header("String value")]
        public GameObject stringHolder;
        public InputField stringInput;

        [Header("Number value")]
        public GameObject numberHolder;
        public InputField numberInput;

        private Attributes attributes;
        private string key;
        private bool init;

        public void CreateFromAttributes(Attributes attributes, string key = "")
        {
            init = true;
            this.attributes = attributes;
            this.key = key ?? "";
            keyInput.text = key ?? "";
            DisableAllHolders();
            if(string.IsNullOrEmpty(key) || !attributes.Contains(key))
            {
                SetType(typeof(bool));
                init = false;
                return;
            }

            var t = GetTypeOf(key);
            if(t == typeof(bool))
            {
                var v = attributes.Get<bool>(key);
                typeInput.value = 0;
                SetType(t);
                (v ? booleanTrue : booleanFalse).isOn = true;
            }
            else if(t == typeof(float))
            {
                var v = attributes.Get<float>(key).ToString();
                typeInput.value = 1;
                SetType(t);
                numberInput.text = v;
            }
            else if(t == typeof(int))
            {
                var v = attributes.Get<int>(key).ToString();
                typeInput.value = 2;
                SetType(t);
                numberInput.text = v;
            }
            else if(t == typeof(string))
            {
                var v = attributes.Get<string>(key);
                typeInput.value = 3;
                SetType(t);
                stringInput.text = v;
            }

            init = false;
        }

        private Type GetTypeOf(string key)
        {
            var types = new []{typeof(bool), typeof(float), typeof(int), typeof(string)};
            return types.FirstOrDefault(t => attributes.IsKeyType(key, t));
        }

        private void SetKey(string k)
        {
            if(k == key)
                return;
            // Clone existing value
            if(!string.IsNullOrEmpty(key) && attributes.Contains(key))
            {
                var t = GetTypeOf(key);
                if(t == typeof(bool))
                    attributes.Set(k, attributes.Get<bool>(key));
                else if(t == typeof(float))
                    attributes.Set(k, attributes.Get<float>(key));
                else if(t == typeof(int))
                    attributes.Set(k, attributes.Get<int>(key));
                else
                    attributes.Set(k, attributes.Get<string>(key));
                attributes.Remove(key);
            }
            // New key
            else
            {
                attributes.Set(k, false);
                typeInput.value = 0;
            }
            // Update key
            key = k;
            keyInput.text = k;
        }

        private void SetType(Type t)
        {
            DisableAllHolders();
            if(t == typeof(bool))
            {
                booleanHolder.SetActive(true);
                booleanFalse.interactable = true;
                booleanTrue.interactable = true;
            }
            else if(t == typeof(float))
            {
                numberHolder.SetActive(true);
                numberInput.contentType = InputField.ContentType.DecimalNumber;
                numberInput.interactable = true;
            }
            else if(t == typeof(int))
            {
                numberHolder.SetActive(true);
                numberInput.contentType = InputField.ContentType.IntegerNumber;
                numberInput.interactable = true;
            }
            else if(t == typeof(string))
            {
                stringHolder.SetActive(true);
                stringInput.interactable = true;
            }
        }

        private void DisableAllHolders()
        {
            booleanHolder.SetActive(false);
            stringHolder.SetActive(false);
            numberHolder.SetActive(false);
        }

        private void ResetAllHolders()
        {
            booleanFalse.Select();
            booleanTrue.interactable = false;
            booleanFalse.interactable = false;

            stringInput.text = "";
            stringInput.interactable = false;

            numberInput.text = "";
            numberInput.interactable = false;
        }

        public void SetTypeDropdown(int type)
        {
            if(init)
                return;
            ResetAllHolders();
            switch(type)
            {
                case 0:
                    SetType(typeof(bool));
                    if(!string.IsNullOrEmpty(key))
                        attributes.Set(key, false);
                    break;
                case 1:
                    SetType(typeof(float));
                    if(!string.IsNullOrEmpty(key))
                        attributes.Set(key, 0.0f);
                    break;
                case 2:
                    SetType(typeof(int));
                    if(!string.IsNullOrEmpty(key))
                        attributes.Set(key, 1);
                    break;
                case 3:
                    SetType(typeof(string));
                    if(!string.IsNullOrEmpty(key))
                        attributes.Set(key, "");
                    break;
            }
        }

        public void RemoveButton()
        {
            if(key != "" && attributes.Contains(key))
                attributes.Remove(key);
            Destroy(gameObject);
        }

        public void SetKeyInput(string newKey)
        {
            if(init)
                return;
            SetKey(newKey);
        }

        public void BooleanInputTrue(bool value)
        {
            if(init)
                return;
            if(!value || string.IsNullOrEmpty(key))
                return;
            attributes.Set(key, true);
        }

        public void BooleanInputFalse(bool value)
        {
            if(init)
                return;
            if(!value || string.IsNullOrEmpty(key))
                return;
            attributes.Set(key, false);
        }

        public void NumberInput(string value)
        {
            if(init)
                return;
            if(string.IsNullOrEmpty(value) || string.IsNullOrEmpty(key))
                return;
            if(GetTypeOf(key) == typeof(float))
                attributes.Set(key, float.Parse(value));
            else if(GetTypeOf(key) == typeof(int))
                attributes.Set(key, int.Parse(value));
        }

        public void StringInput(string value)
        {
            if(init)
                return;
            if(string.IsNullOrEmpty(value) || string.IsNullOrEmpty(key))
                return;
            attributes.Set(key, value);
        }

    }
}