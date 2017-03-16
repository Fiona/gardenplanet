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
				GUILayout.Box("Debug");
				if(GUILayout.Button("Open"))
					DebugMode = 1;
			}

			// Debug submenu selection
			if(DebugMode == 1)
			{
				GUILayout.Box("Debug");

				if(GUILayout.Button("World timer"))
					DebugMode = 2;

				if(GUILayout.Button("< Back"))
					DebugMode = 0;
			}

			// World timer
			if(DebugMode == 2)
			{
				GUILayout.Box("World timer");

				var worldTimer = FindObjectOfType<WorldTimer>();

				if(GUILayout.Button("Next hour"))
					worldTimer.GoToNextHour();

				if(GUILayout.Button("Next day"))
					worldTimer.GoToNextDay();

				if(GUILayout.Button("Next season"))
					worldTimer.GoToNextSeason();

				if(GUILayout.Button("Next year"))
					worldTimer.GoToNextYear();

				if(GUILayout.Button("< Back"))
					DebugMode = 1;				
			}

			GUILayout.EndArea();

		}
				
	}
}

