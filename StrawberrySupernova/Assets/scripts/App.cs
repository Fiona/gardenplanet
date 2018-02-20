using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StrawberryNova
{

	public class App: MonoBehaviour
	{

	    [HideInInspector]
	    public AppState state;

		public static GameSettings gameSettings;

	    public void Awake()
	    {
			DontDestroyOnLoad(this);
		    App.gameSettings = new GameSettings();
			StartNewState(Consts.INITIAL_APP_STATE);
	    }

		public void Quit()
		{
			Application.Quit();
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
		}

	    public void StartNewState(AppState state)
	    {
	        this.state = state;
	        switch(state)
	        {
		        case AppState.Title:
			        SceneManager.LoadScene("titlescreen");
			        break;
		        case AppState.Editor:
			        SceneManager.LoadScene("mapeditor");
			        break;
		        case AppState.Game:
			        SceneManager.LoadScene("game");
			        break;
	        }
	    }

	}

}