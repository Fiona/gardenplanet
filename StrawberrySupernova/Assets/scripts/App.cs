using System;
using System.Collections;
using StompyBlondie;
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
            var canvas = FindObjectOfType<Canvas>();
            if(canvas == null)
            {
                SceneManager.LoadScene(sceneName);
                yield break;
            }

            var loadingScreenObj = Instantiate(Resources.Load(Consts.PREFAB_PATH_LOADING_SCREEN)) as GameObject;
            loadingScreenObj.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            var canvasGroup = loadingScreenObj.GetComponent<CanvasGroup>();

            yield return StartCoroutine(LerpHelper.QuickFadeIn(canvasGroup, .5f, lerpType: LerpHelper.Type.SmoothStep));

            var asyncLoad = SceneManager.LoadSceneAsync(sceneName);
            asyncLoad.allowSceneActivation = false;

            while(!asyncLoad.isDone)
            {
                if(asyncLoad.progress >= 0.9f)
                {
                    yield return StartCoroutine(LerpHelper.QuickFadeOut(canvasGroup, .5f, lerpType: LerpHelper.Type.SmoothStep));
                    yield return new WaitForSeconds(0.1f);
                    asyncLoad.allowSceneActivation = true;
                }
                yield return new WaitForFixedUpdate();
            }
        }
    }

}