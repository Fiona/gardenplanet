using System;
using UnityEngine;
using Tayx.Graphy;

namespace GardenPlanet
{
    public class DebugMenu: MonoBehaviour
    {
        static int DebugMode;
        private static bool graphyOn;
        private const int width = 150;
        private GraphyManager graphy;

        void Start()
        {
            graphy = FindObjectOfType<GraphyManager>();
            GoAwayGraphy();
        }

        void OnGUI()
        {

            GUILayout.BeginArea(new Rect(Screen.width - width - 20, 20, width, 300));

            // Root
            if(DebugMode == 0)
            {
                //GUILayout.Box("Debug");
                if(GUILayout.Button("Debug Menu"))
                    DebugMode = 1;
            }

            // Debug submenu selection
            if(DebugMode == 1)
            {
                GUILayout.Box("Debug");

                if(GUILayout.Button("App"))
                    DebugMode = 99;

                if(GUILayout.Button("World timer"))
                    DebugMode = 2;

                if(GUILayout.Button("Items"))
                    DebugMode = 3;

                if(GUILayout.Button("Print info"))
                    DebugMode = 4;

                if(GUILayout.Button("Graphy Toggle"))
                {
                    graphyOn = !graphyOn;
                    GoAwayGraphy();
                    if(graphyOn)
                    {
                        graphy.FpsModuleState = GraphyManager.ModuleState.FULL;
                        graphy.RamModuleState = GraphyManager.ModuleState.FULL;
                        graphy.AdvancedModuleState = GraphyManager.ModuleState.FULL;
                        var listener = FindObjectOfType<AudioListener>();
                        graphy.AudioListener = listener;
                        graphy.AudioModuleState =
                            listener ? GraphyManager.ModuleState.FULL : GraphyManager.ModuleState.OFF;
                    }
                }

                if(GUILayout.Button("< Back"))
                    DebugMode = 0;
            }

            // World timer
            if(DebugMode == 2)
            {
                GUILayout.Box("World timer");

                var worldTimer = FindObjectOfType<WorldTimer>();

                if(GUILayout.Button("Next hour"))
                    worldTimer.gameTime += new GameTime(hours: 1);

                if(GUILayout.Button("Next day"))
                    worldTimer.gameTime += new GameTime(days: 1);

                if(GUILayout.Button("Next season"))
                    worldTimer.gameTime += new GameTime(seasons: 1);

                if(GUILayout.Button("Next year"))
                    worldTimer.gameTime += new GameTime(years: 1);

                if(GUILayout.Button("< Back"))
                    DebugMode = 1;
            }

            // Items
            if(DebugMode == 3)
            {
                GUILayout.Box("Items");

                var itemManager = FindObjectOfType<ItemManager>();

                if(GUILayout.Button("Give Tools Level 1"))
                {
                    itemManager.GivePlayerItem("wateringcan-level1");
                    itemManager.GivePlayerItem("hoe-level1");
                }

                if(GUILayout.Button("Give Seeds"))
                {
                    itemManager.GivePlayerItem("cabbage_seeds", quantity:16);
                    itemManager.GivePlayerItem("turnip_seeds", quantity:16);
                }

                if(GUILayout.Button("Give Vegetables"))
                {
                    itemManager.GivePlayerItem("cabbage", quantity: 16);
                }

                if(GUILayout.Button("< Back"))
                    DebugMode = 1;
            }

            // Print info
            if(DebugMode == 4)
            {
                GUILayout.Box("Print info");

                var controller = FindObjectOfType<GameController>();

                if(GUILayout.Button("Player Inventory"))
                {
                    Debug.Log($"Num items: {controller.world.player.inventory.Items.Count}");
                    foreach(var item in controller.world.player.inventory.Items)
                        Debug.Log(item.ToString());
                }

                if(GUILayout.Button("< Back"))
                    DebugMode = 1;
            }

            // App
            if(DebugMode == 99)
            {
                GUILayout.Box("App");

                if(GUILayout.Button("To Logo"))
                    FindObjectOfType<App>().StartNewState(AppState.Logo);

                if(GUILayout.Button("To Title"))
                    FindObjectOfType<App>().StartNewState(AppState.Title);

                if(GUILayout.Button("To Game"))
                    FindObjectOfType<App>().StartNewState(AppState.Game);

                if(GUILayout.Button("To Editor"))
                    FindObjectOfType<App>().StartNewState(AppState.Editor);

                if(GUILayout.Button("Quit"))
                {
                    Application.Quit();
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                }
                if(GUILayout.Button("< Back"))
                    DebugMode = 1;
            }

            GUILayout.EndArea();

        }

        void GoAwayGraphy()
        {
            graphy.FpsModuleState = GraphyManager.ModuleState.OFF;
            graphy.RamModuleState = GraphyManager.ModuleState.OFF;
            graphy.AdvancedModuleState = GraphyManager.ModuleState.OFF;
            graphy.AudioModuleState = GraphyManager.ModuleState.OFF;
        }

    }
}