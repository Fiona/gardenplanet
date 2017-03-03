using UnityEngine;

public class TitleScreenController : MonoBehaviour
{

    public void NewGameButtonPressed()
    {
        FindObjectOfType<App>().StartNewState(AppState.Game);
    }

    public void EditorButtonPressed()
    {
        FindObjectOfType<App>().StartNewState(AppState.Editor);
    }

    public void QuitButtonPressed()
    {
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

}
