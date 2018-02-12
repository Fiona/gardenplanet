using StompyBlondie;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

namespace StrawberryNova
{
    public class SettingsCategoryButton: GlobalSelectableButton
    {
        private InGameMenuPageSettings settings;

        public new void Start()
        {
            base.Start();
            settings = FindObjectOfType<InGameMenuPageSettings>();
       }

        protected override void Click(BaseEventData data)
        {
            if(settings.openingPage)
                return;
            base.Click(data);
        }

    }
}