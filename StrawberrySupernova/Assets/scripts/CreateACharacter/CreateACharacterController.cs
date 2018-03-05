using System.Collections;
using System.IO;
using LitJson;
using StompyBlondie;
using TMPro;
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
        public RectTransform page3;

        [Header("Page 1 references")]
        public GlobalButton backToTitleButton;
        public GlobalButton page1NextPageButton;
        public GlobalButton page1RandomButton;
        public TMP_InputField nameField;

        [Header("Page 2 references")]
        public GlobalButton page2NextPageButton;
        public GlobalButton page2PreviousPageButton;
        public GlobalButton page2RandomButton;

        [Header("Page 3 references")]
        public GlobalButton finishButton;
        public GlobalButton page3PreviousPageButton;
        public GlobalButton page3RandomButton;

        [HideInInspector]
        public JsonData globalConfig;

        private int page;
        private Vector2 offScreenPagePosition;

        public void Start()
        {

            // Load global config
            var configFilePath = Path.Combine(Consts.DATA_DIR, Consts.FILE_GLOBAL_CONFIG);
            var jsonContents = "{}";
            if(File.Exists(configFilePath))
                using(var fh = File.OpenText(configFilePath))
                    jsonContents = fh.ReadToEnd();
            globalConfig = JsonMapper.ToObject(jsonContents);

            input.directInputEnabled = false;
            StartCoroutine(OpenPage1Animation());

            // Page 1 callbacks
            backToTitleButton.SetCallback(BackToTitleButtonPressed);
            page1NextPageButton.SetCallback(NextPageButtonPressed);

            nameField.onValueChanged.AddListener((v) => { character.SetName(v); });

            // Page 2 callbacks
            page2PreviousPageButton.SetCallback(PreviousPageButtonPressed);
            page2NextPageButton.SetCallback(NextPageButtonPressed);

            // Page 3 callbacks
            finishButton.SetCallback(FinishButtonPressed);
            page3PreviousPageButton.SetCallback(PreviousPageButtonPressed);
        }

        public void Update()
        {
            // Hiding/removing the next page button depending on name
            if(page == 1)
                page1NextPageButton.gameObject.SetActive(character.GetInformation().Name.Trim() != "");
        }

        public IEnumerator OpenPage1Animation()
        {
            page = 1;

            page1.gameObject.SetActive(true);
            page2.gameObject.SetActive(false);
            page3.gameObject.SetActive(false);

            var screenWidth = FindObjectOfType<Canvas>().GetComponent<RectTransform>().sizeDelta.x;
            offScreenPagePosition = new Vector2(screenWidth, 0f);
            page1.anchoredPosition = offScreenPagePosition;
            page2.anchoredPosition = offScreenPagePosition;
            page3.anchoredPosition = offScreenPagePosition;

            yield return screenFade.FadeIn(fadeInTime);
            yield return new WaitForSeconds(1f);

            yield return LerpHelper.QuickTween(
                (v) => page1.anchoredPosition = v,
                offScreenPagePosition,
                new Vector2(pageXPosition, 0f),
                pageMoveInTime,
                lerpType: LerpHelper.Type.SmoothStep
            );
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
        // PAGE SWITCHING CALLBACKS
        // --------------------------------------------------

        private void BackToTitleButtonPressed()
        {
            StartCoroutine(screenFade.FadeOut(callback: () => FindObjectOfType<App>().StartNewState(AppState.Title)));
        }

        private void NextPageButtonPressed()
        {
            RectTransform fromPage = null, toPage = null;
            int num = 0;
            switch(page)
            {
                case 1:
                    fromPage = page1;
                    toPage = page2;
                    num = 2;
                    break;
                case 2:
                    fromPage = page2;
                    toPage = page3;
                    num = 3;
                    break;
            }
            StartCoroutine(SwitchPageAnimation(fromPage, toPage, num));
        }

        private void PreviousPageButtonPressed()
        {
            RectTransform fromPage = null, toPage = null;
            int num = 0;
            switch(page)
            {
                case 2:
                    fromPage = page2;
                    toPage = page1;
                    num = 1;
                    break;
                case 3:
                    fromPage = page3;
                    toPage = page2;
                    num = 2;
                    break;
            }
            StartCoroutine(SwitchPageAnimation(fromPage, toPage, num));
        }

        private void FinishButtonPressed()
        {
            StartCoroutine(screenFade.FadeOut(callback: () => FindObjectOfType<App>().StartNewState(AppState.Game)));
        }

    }
}