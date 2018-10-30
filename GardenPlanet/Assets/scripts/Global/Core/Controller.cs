using System.IO;
using UnityEngine;

namespace GardenPlanet
{
    public class Controller: MonoBehaviour
    {
        [HideInInspector]
        public GlobalConfig globalConfig;

        protected GameState gameState;

        protected virtual void Awake()
        {
            // Grab current game state
            gameState = GameState.GetInstance();

            // Load global config
            var configFilePath = Path.Combine(Consts.DATA_DIR, Consts.FILE_GLOBAL_CONFIG);
            var jsonContents = "{}";
            if(File.Exists(configFilePath))
                using(var fh = File.OpenText(configFilePath))
                    jsonContents = fh.ReadToEnd();
            globalConfig = new GlobalConfig();
            JsonHandler.PopulateObject(jsonContents, globalConfig);
        }
    }
}