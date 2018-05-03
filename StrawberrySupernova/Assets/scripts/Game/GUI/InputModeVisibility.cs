using StompyBlondie;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.UI;

namespace GardenPlanet
{
    [RequireComponent(typeof(CanvasGroup))]
    public class InputModeVisibility : MonoBehaviour
    {
        public bool appearInMouseModeOnly = true;
        public float fadeSpeed = .25f;

        private GameInputManager inputManager;
        private CanvasGroup canvasGroup;
        private bool showing;

        private void Start()
        {
            inputManager = FindObjectOfType<GameInputManager>();
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0f;
            showing = false;
        }

        private void Update()
        {
            if(appearInMouseModeOnly)
            {
                if(inputManager.mouseMode && !showing)
                {
                    Show();
                    return;
                }

                if(!inputManager.mouseMode && showing)
                {
                    Hide();
                    return;
                }
            }
            else
            {
                if(!inputManager.mouseMode && !showing)
                {
                    Show();
                    return;
                }

                if(inputManager.mouseMode && showing)
                {
                    Hide();
                    return;
                }
            }
        }

        private void Show()
        {
            StartCoroutine(LerpHelper.QuickFadeIn(canvasGroup, fadeSpeed, LerpHelper.Type.SmoothStep));
            showing = true;
        }

        private void Hide()
        {
            StartCoroutine(LerpHelper.QuickFadeOut(canvasGroup, fadeSpeed, LerpHelper.Type.SmoothStep));
            showing = false;
        }

    }
}