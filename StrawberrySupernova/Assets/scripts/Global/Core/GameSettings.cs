using System.Collections;
using System.IO;
using System.Text;
using LitJson;
using UnityEngine;

namespace StrawberryNova
{
    public class GameSettings
    {

        public bool popupInfoStatic;

        private string gameSettingsPath;
        private JsonData settings;

        public GameSettings()
        {
            Load();
            Save();
        }

        private void Load()
        {
            gameSettingsPath = Path.Combine(Application.persistentDataPath, Consts.FILE_GAME_SETTINGS_FILE);
            var jsonContents = "{}";
            if(File.Exists(gameSettingsPath))
                using(var fh = File.OpenText(gameSettingsPath))
                    jsonContents = fh.ReadToEnd();
            settings = JsonMapper.ToObject(jsonContents);

            // Load settings
            popupInfoStatic = (bool)(SettingContain("popupInfoStatic") ? settings["popupInfoStatic"] : false);
        }

        public void Save()
        {
            settings["popupInfoStatic"] = popupInfoStatic;

            string jsonOutput;
            jsonOutput = JsonMapper.ToJson(settings);

            using(var fh = File.OpenWrite(gameSettingsPath))
            {
                var jsonBytes = Encoding.UTF8.GetBytes(jsonOutput);
                fh.SetLength(0);
                fh.Write(jsonBytes, 0, jsonBytes.Length);
            }
        }

        private bool SettingContain(string key)
        {
            if(settings == null || !settings.IsObject)
                return false;
            var tdictionary = settings as IDictionary;
            if(tdictionary == null)
                return false;
            return tdictionary.Contains(key);
        }
    }
}