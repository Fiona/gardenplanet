using System;
using UnityEngine;

namespace GardenPlanet
{
    public class EditAttributesDialog: MonoBehaviour
    {
        private Attributes workingAttributes;
        private Action cancelCallback;
        private Action<Attributes> applyCallback;

        public void EditAttributes(Attributes attributes, Action cancelCallback, Action<Attributes> applyCallback)
        {
            workingAttributes = new Attributes(attributes);
            this.applyCallback = applyCallback;
            this.cancelCallback = cancelCallback;
        }

        public void CancelButton()
        {
            cancelCallback?.Invoke();
        }

        public void ApplyButton()
        {
            applyCallback?.Invoke(new Attributes(workingAttributes));
        }
    }
}