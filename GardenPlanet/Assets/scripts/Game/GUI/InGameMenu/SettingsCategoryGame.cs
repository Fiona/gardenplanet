using System.Collections;
using System.Xml.Serialization;
using StompyBlondie;
using UnityEngine;

namespace GardenPlanet
{
    public class SettingsCategoryGame: SettingsCategory
    {
        [Header("Settings objects")]
        public GlobalCheckboxGroup popupInfoStatic;
        public GlobalCheckboxGroup autoPickupItems;

        public override void Init(GUINavigator catButtonNavigator)
        {
            popupInfoStatic.SetValue(App.gameSettings.popupInfoStatic ? 1 : 0);
            autoPickupItems.SetValue(App.gameSettings.autoPickupItems ? 1 : 0);
            base.Init(catButtonNavigator);
        }

        public override IEnumerator Open()
        {
            categoryButtonNavigator.nextLinkedNavigator = popupInfoStatic.navigator;
            popupInfoStatic.navigator.previousLinkedNavigator = categoryButtonNavigator;

            popupInfoStatic.navigator.nextLinkedNavigator = autoPickupItems.navigator;
            autoPickupItems.navigator.previousLinkedNavigator = popupInfoStatic.navigator;
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
            App.gameSettings.autoPickupItems = autoPickupItems.GetValue() == 1;
            App.gameSettings.Save();
            yield return null;
        }

    }
}