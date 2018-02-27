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
            SceneManager.LoadScene("loading");
            var sceneName = "";
            switch(state)
            {
                case AppState.Title:
                    sceneName = "titlescreen";
                    break;
                case AppState.Editor:
                    sceneName = "mapeditor";
                    break;
                case AppState.Game:
                    sceneName = "game";
                    break;
                case AppState.CreateACharacter:
                    sceneName = "createacharacter";
                    break;
            }

            StartCoroutine(LoadScene(sceneName));
        }

        public IEnumerator LoadScene(string sceneName)
        {
            //yield return new WaitForSeconds(3);

            var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            while(!asyncLoad.isDone)
            {
                if(asyncLoad.progress == 0.9f)
                    asyncLoad.allowSceneActivation = true;
                yield return new WaitForFixedUpdate();
            }
        }
    }

}