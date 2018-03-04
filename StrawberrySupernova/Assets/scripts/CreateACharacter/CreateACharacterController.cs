using System.Collections;
using StompyBlondie;
using UnityEngine;

namespace StrawberryNova
{
    public class CreateACharacterController: MonoBehaviour
    {

        [Header("Settings")]
        public float fadeInTime;
        public float fadeOutTime;
        public float pageXPosition = 50f;
        public float pageMoveInTime;
        public float pageMoveOutTime;

        [Header("Object references")]
        public GameInputManager input;
        public ScreenFade screenFade;
        public CACCharacter character;
        public RectTransform page1;
        public RectTransform page2;

        [Header("Page 1 references")]
        public GlobalButton backToTitleButton;
        public GlobalButton nextPageButton;
        public GlobalButton page1RandomButton;

        [Header("Page 2 references")]
        public GlobalButton finishButton;
        public GlobalButton previousPageButton;
        public GlobalButton page2RandomButton;

        private int page;
        private Vector2 offScreenPagePosition;

        public void Start()
        {
            input.directInputEnabled = false;
            StartCoroutine(OpenPage1Animation());

            // Page 1 callbacks
            backToTitleButton.SetCallback(BackToTitleButtonPressed);
            nextPageButton.SetCallback(NextPageButtonPressed);

            // Page 2 callbacks
            finishButton.SetCallback(FinishButtonPressed);
            previousPageButton.SetCallback(PreviousPageButtonPressed);
        }

        public IEnumerator OpenPage1Animation()
        {
            page1.gameObject.SetActive(true);
            page2.gameObject.SetActive(false);

            var screenWidth = FindObjectOfType<Canvas>().GetComponent<RectTransform>().sizeDelta.x;
            offScreenPagePosition = new Vector2(screenWidth, 0f);
            page1.anchoredPosition = offScreenPagePosition;
            page2.anchoredPosition = offScreenPagePosition;

            yield return screenFade.FadeIn(fadeInTime);
            yield return new WaitForSeconds(1f);

            yield return LerpHelper.QuickTween(
                (v) => page1.anchoredPosition = v,
                offScreenPagePosition,
                new Vector2(pageXPosition, 0f),
                pageMoveInTime,
                lerpType: LerpHelper.Type.SmoothStep
                );

            page = 1;
        }

        public IEnumerator SwitchPageAnimation(RectTransform fromPage, RectTransform toPage, int num)
        {
            yield return LerpHelper.QuickTween(
                (v) => fromPage.anchoredPosition = v,
                new Vector2(pageXPosition, 0f),
                new Vector2(-fromPage.sizeDelta.x, 0f),
                pageMoveOutTime,
                lerpType: LerpHelper.Type.SmoothStep
            );
            fromPage.gameObject.SetActive(false);

            toPage.gameObject.SetActive(true);
            yield return LerpHelper.QuickTween(
                (v) => toPage.anchoredPosition = v,
                offScreenPagePosition,
                new Vector2(pageXPosition, 0f),
                pageMoveInTime,
                lerpType: LerpHelper.Type.SmoothStep
            );

            page = num;
        }

        // --------------------------------------------------
        // PAGE 1 CALLBACKS
        // --------------------------------------------------

        private void BackToTitleButtonPressed()
        {
            StartCoroutine(screenFade.FadeOut(callback: () => FindObjectOfType<App>().StartNewState(AppState.Title)));
        }

        private void NextPageButtonPressed()
        {
            StartCoroutine(SwitchPageAnimation(page1, page2, 2));
        }

        // --------------------------------------------------
        // PAGE 2 CALLBACKS
        // --------------------------------------------------

        private void PreviousPageButtonPressed()
        {
            StartCoroutine(SwitchPageAnimation(page2, page1, 1));
        }

        private void FinishButtonPressed()
        {
            StartCoroutine(screenFade.FadeOut(callback: () => FindObjectOfType<App>().StartNewState(AppState.Game)));
        }

    }
}