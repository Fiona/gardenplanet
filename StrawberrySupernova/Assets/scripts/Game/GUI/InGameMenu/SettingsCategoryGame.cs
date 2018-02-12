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

        public override void Init(GUINavigator catButtonNavigator)
        {
            popupInfoStatic.SetValue(App.gameSettings.popupInfoStatic ? 1 : 0);
            base.Init(catButtonNavigator);
        }

        public override IEnumerator Open()
        {
            categoryButtonNavigator.nextLinkedNavigator = popupInfoStatic.navigator;
            popupInfoStatic.navigator.previousLinkedNavigator = categoryButtonNavigator;
            yield return base.Open();
        }

        public override IEnumerator Close()
        {
            categoryButtonNavigator.nextLinkedNavigator = null;
            yield return base.Close();
        }

        public override IEnumerator Save()
        {
            App.gameSettings.popupInfoStatic = popupInfoStatic.GetValue() == 1;
            App.gameSettings.Save();
            yield return null;
        }

    }
}