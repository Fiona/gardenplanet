using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace GardenPlanet
{
    public class PlayerSettings
    {

        public bool popupInfoStatic;
        public bool autoPickupItems;

        private string playerSettingsPath;

        public PlayerSettings()
        {
            Load();
            Save();
        }

        private void Load()
        {
            playerSettingsPath = Path.Combine(Application.persistentDataPath, Consts.FILE_PLAYER_SETTINGS_FILE);
            var jsonContents = "{}";
            if(File.Exists(playerSettingsPath))
                using(var fh = File.OpenText(playerSettingsPath))
                    jsonContents = fh.ReadToEnd();
            JsonHandler.PopulateObject(jsonContents, this);

            // Load settings
            //popupInfoStatic = (bool)(SettingContain("popupInfoStatic") ? settings["popupInfoStatic"] : false);
            //autoPickupItems = (bool)(SettingContain("autoPickupItems") ? settings["autoPickupItems"] : true);
        }

        public void Save()
        {
            var jsonOutput = JsonHandler.Serialize(this);
            using(var fh = File.OpenWrite(playerSettingsPath))
            {
                var jsonBytes = Encoding.UTF8.GetBytes(jsonOutput);
                fh.SetLength(0);
                fh.Write(jsonBytes, 0, jsonBytes.Length);
            }
        }

        /*
        private bool SettingContain(string key)
        {
            if(settings == null || !settings.IsObject)
                return false;
            var tdictionary = settings as IDictionary;
            if(tdictionary == null)
                return false;
            return tdictionary.Contains(key);
        }*/
    }
}