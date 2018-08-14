using System;
using Boo.Lang;
using UnityEngine;

namespace GardenPlanet
{
    public class EditAttributesDialog: MonoBehaviour
    {
        public AttributeField attributeFieldTemplate;
        public GameObject attributeHolder;

        private Attributes workingAttributes;
        private Action cancelCallback;
        private Action<Attributes> applyCallback;

        public void EditAttributes(Attributes attributes, Action cancelCallback, Action<Attributes> applyCallback)
        {
            workingAttributes = new Attributes(attributes);
            this.applyCallback = applyCallback;
            this.cancelCallback = cancelCallback;
            foreach(var attr in attributes)
                AddAttributeField(attr.Key.ToString());
            attributeFieldTemplate.gameObject.SetActive(false);
        }

        public void CancelButton()
        {
            cancelCallback?.Invoke();
        }

        public void ApplyButton()
        {
            applyCallback?.Invoke(new Attributes(workingAttributes));
        }

        public void AddAttributeButton()
        {
            AddAttributeField("");
        }

        private void AddAttributeField(string key)
        {
            var newAttribute = Instantiate(attributeFieldTemplate);
            newAttribute.gameObject.SetActive(true);
            newAttribute.transform.SetParent(attributeHolder.transform, false);
            newAttribute.CreateFromAttributes(workingAttributes, key);
        }

    }
}