using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace StrawberryNova
{
	
	public class App: MonoBehaviour
	{

	    [HideInInspector]
	    public AppState state;
		[HideInInspector]
		public Events events;

	    public void Awake()
	    {
			DontDestroyOnLoad(this);
			events = new Events();
			StartNewState(Consts.INITIAL_APP_STATE);
	    }

	    public void StartNewState(AppState state)
	    {
	        this.state = state;
	        if(state == AppState.Title)
	            SceneManager.LoadScene("titlescreen");
	        if(state == AppState.Editor)
	            SceneManager.LoadScene("mapeditor");
	        if(state == AppState.Game)
	            SceneManager.LoadScene("game");
	    }

	}

}