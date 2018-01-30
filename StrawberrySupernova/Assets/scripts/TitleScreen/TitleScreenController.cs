using System.Collections;
using System.Linq;
using Rewired;
using Rewired.Editor;
using StompyBlondie;
using UnityEngine;

namespace StrawberryNova
{

    public class TitleScreenController : MonoBehaviour
    {

        [Header("Settings")]
        public float buttonAnimateTime;
        public float offScreenXPos;
        public float fadeInTime;
        public float fadeOutTime;

        [Header("Object references")]
        public GameInputManager input;
        public ScreenFade screenFade;

        [Header("Button references")]
        public GlobalButton newGameButton;
        public GlobalButton loadGameButton;
        public GlobalButton optionsButton;
        public GlobalButton quitButton;
        public GlobalButton editorButton;

        private GUINavigator navigator;

        public void Update()
        {
        }

        public void Start()
        {
            input.directInputEnabled = false;

            newGameButton.SetCallback(NewGameButtonPressed);
            editorButton.SetCallback(EditorButtonPressed);
            quitButton.SetCallback(QuitButtonPressed);

            StartCoroutine(TitleAnimation());
        }

        public IEnumerator TitleAnimation()
        {
            var buttons = new GlobalButton[]{newGameButton,loadGameButton,optionsButton,quitButton};
            bool dir = true;
            foreach(var b in buttons)
            {
                // Position buttons off screen
                b.rectTransform.anchoredPosition = new Vector2(
                    dir ? -offScreenXPos : offScreenXPos,
                    b.rectTransform.anchoredPosition.y
                );
                dir = !dir;
            }

            yield return screenFade.FadeIn(fadeInTime);

            // animate in buttons
            foreach(var b in buttons)
            {
                AnimateButton(b);
                yield return new WaitForSeconds(buttonAnimateTime/2);
            }

            // Set up navigation
            navigator = gameObject.AddComponent<GUINavigator>();
            foreach(var b in buttons)
                navigator.AddNavigationElement(b.rectTransform, buttons.Last() == b);
        }

        public void NewGameButtonPressed()
        {
            StartCoroutine(
                screenFade.FadeOut(fadeOutTime, callback: () => FindObjectOfType<App>().StartNewState(AppState.Game))
            );
        }

        public void EditorButtonPressed()
        {
            FindObjectOfType<App>().StartNewState(AppState.Editor);
        }

        public void QuitButtonPressed()
        {
            StartCoroutine(
                screenFade.FadeOut(fadeOutTime, callback: () =>
                    {
                        Application.Quit();
#if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
#endif
                    }
                )
            );
        }

        private void AnimateButton(GlobalButton button)
        {
            StartCoroutine(LerpHelper.QuickTween(
                (v) => button.rectTransform.anchoredPosition = v,
                button.rectTransform.anchoredPosition,
                new Vector2(0f, button.rectTransform.anchoredPosition.y),
                buttonAnimateTime,
                lerpType:LerpHelper.Type.BounceOut
            ));
        }

    }

}