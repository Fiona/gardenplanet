using System;
using System.Collections;
using StompyBlondie;
using Tayx.Graphy;
using UnityEngine;
using UnityEngine.SceneManagement;
using StompyBlondie.Utils;

namespace GardenPlanet
{

    public class App: MonoBehaviour
    {
        public AppSettings appSettings;
        public Canvas appCanvas;
        public static PlayerSettings PlayerSettings;
        [HideInInspector]
        public AppState state;

        public static App Instance => _instance;
        private static App _instance;

        public void Awake()
        {
            _instance = this;
            DontDestroyOnLoad(this);
            App.PlayerSettings = new PlayerSettings();
            StartNewState(appSettings.InitialAppState);

            // Optional debug menu
            if(appSettings.ShowDebugMenu)
            {
                var debugObj = new GameObject("DebugMenu");
                debugObj.AddComponent<DebugMenu>();
                DontDestroyOnLoad(debugObj);
            }
            else
                Destroy(FindObjectOfType<GraphyManager>()?.gameObject);
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
                case AppState.Logo:
                    sceneName = "logo";
                    break;
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
            if(appCanvas == null)
            {
                SceneManager.LoadScene(sceneName);
                yield break;
            }

            var loadingScreenObj = Instantiate(Resources.Load(Consts.PREFAB_PATH_LOADING_SCREEN)) as GameObject;
            loadingScreenObj.transform.SetParent(appCanvas.transform, false);
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