using UnityEngine;
using UnityEngine.Serialization;

namespace GardenPlanet
{
    [CreateAssetMenu(fileName = "App", menuName = "Config/App Settings", order = 1)]
    public class AppSettings: ScriptableObject
    {
        [Header("Start Up Settings")]
        [Tooltip("Which application state to boot into")]
        public AppState InitialAppState = AppState.Logo;

        [Tooltip("Which map to load into first")]
        public string StartMap = "devtest";

        [Header("Debug Settings")]
        [Tooltip("Top right debug menu should be shown")]
        public bool ShowDebugMenu;

        [Tooltip("If the AI task manager should constantly dump to the console")]
        public bool AITasksShouldOutput;
    }
}