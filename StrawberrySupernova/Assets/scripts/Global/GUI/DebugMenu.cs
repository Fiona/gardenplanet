using System;
using UnityEngine;

namespace StrawberryNova
{
	public class DebugMenu: MonoBehaviour
	{
		static int DebugMode;
		const int width = 150;

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
		            itemManager.GivePlayerItem("broken_sprinkleboy");
		            itemManager.GivePlayerItem("broken_trowelie");
	            }

                if(GUILayout.Button("Give Seeds"))
                {
	                itemManager.GivePlayerItem("turnip_seeds", quantity:16);
                }

                if(GUILayout.Button("< Back"))
                    DebugMode = 1;
            }

            // App
            if(DebugMode == 99)
            {
                GUILayout.Box("App");

                if(GUILayout.Button("To Title"))
                    FindObjectOfType<App>().StartNewState(AppState.Title);

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

	}
}

