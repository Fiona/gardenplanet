using System.Collections;
using System.Xml.Serialization;
using StompyBlondie;
using UnityEngine;

namespace StrawberryNova
{
    public class SettingsCategoryGame: SettingsCategory
    {
        [Header("Settings objects")]
        public GlobalCheckboxGroup popupInfoStatic;

        public override void Init()
        {
            popupInfoStatic.SetValue(App.gameSettings.popupInfoStatic ? 1 : 0);
            base.Init();
        }

        public override IEnumerator Save()
        {
            App.gameSettings.popupInfoStatic = popupInfoStatic.GetValue() == 1;
            App.gameSettings.Save();
            yield return null;
        }

    }
}