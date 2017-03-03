using UnityEngine;
using UnityEngine.SceneManagement;

public class App: MonoBehaviour
{

    [HideInInspector]
    public AppState state;

    public void Awake()
    {
        DontDestroyOnLoad(this);
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
