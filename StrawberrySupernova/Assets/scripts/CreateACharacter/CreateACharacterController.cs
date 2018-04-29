using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using LitJson;
using StompyBlondie;
using TMPro;
using UnityEngine;

namespace StrawberryNova
{
    [System.Serializable]
    public class MultiDimensionalRectTransform
    {
        public RectTransform[] rectTransformArray;
    }

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
        public AppearenceField[] page1RandomisableFields;

        [Header("Page 2 references")]
        public GlobalButton page2NextPageButton;
        public GlobalButton page2PreviousPageButton;
        public GlobalButton page2RandomButton;
        public AppearenceField[] page2RandomisableFields;

        [Header("Page 3 references")]
        public GlobalButton finishButton;
        public GlobalButton page3PreviousPageButton;
        public GlobalButton page3RandomButton;
        public AppearenceField[] page3RandomisableFields;

        [Header("Navigation Targets")]
        public MultiDimensionalRectTransform[] page1NavigationTargets;
        public MultiDimensionalRectTransform[] page2NavigationTargets;
        public MultiDimensionalRectTransform[] page3NavigationTargets;

        [HideInInspector]
        public JsonData globalConfig;

        private int page;
        private Vector2 offScreenPagePosition;
        private bool switchingPages;
        private List<GUINavigator> fieldNavigators;

        public void Awake()
        {
            fieldNavigators = new List<GUINavigator>();

            // Load global config
            var configFilePath = Path.Combine(Consts.DATA_DIR, Consts.FILE_GLOBAL_CONFIG);
            var jsonContents = "{}";
            if(File.Exists(configFilePath))
                using(var fh = File.OpenText(configFilePath))
                    jsonContents = fh.ReadToEnd();
            globalConfig = JsonMapper.ToObject(jsonContents);
        }

        public void Start()
        {
            input.directInputEnabled = false;
            character.SetName("");
            StartCoroutine(OpenPage1Animation());

            // Page 1 callbacks
            backToTitleButton.SetCallback(BackToTitleButtonPressed);
            page1NextPageButton.SetCallback(NextPageButtonPressed);
            page1RandomButton.SetCallback(RandomizeButton);

            nameField.onValueChanged.AddListener((v) => { character.SetName(v); });

            // Page 2 callbacks
            page2PreviousPageButton.SetCallback(PreviousPageButtonPressed);
            page2NextPageButton.SetCallback(NextPageButtonPressed);
            page2RandomButton.SetCallback(RandomizeButton);

            // Page 3 callbacks
            finishButton.SetCallback(FinishButtonPressed);
            finishButton.repeatable = false;
            page3PreviousPageButton.SetCallback(PreviousPageButtonPressed);
            page3RandomButton.SetCallback(RandomizeButton);
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

            var screenWidth = FindObjectOfType<Canvas>().GetComponent<RectTransform>().sizeDelta.x;
            offScreenPagePosition = new Vector2(screenWidth, 0f);
            page1.anchoredPosition = offScreenPagePosition;
            page2.anchoredPosition = offScreenPagePosition;
            page3.anchoredPosition = offScreenPagePosition;

            // Want them all active to trigger Start functions
            page1.gameObject.SetActive(true);
            page2.gameObject.SetActive(true);
            page3.gameObject.SetActive(true);

            // Wait a frame for the character objects to be generated
            yield return new WaitForFixedUpdate();
            RandomizeAll();

            yield return screenFade.FadeIn(fadeInTime);
            yield return new WaitForSeconds(1f);

            page2.gameObject.SetActive(false);
            page3.gameObject.SetActive(false);

            yield return LerpHelper.QuickTween(
                (v) => page1.anchoredPosition = v,
                offScreenPagePosition,
                new Vector2(pageXPosition, 0f),
                pageMoveInTime,
                lerpType: LerpHelper.Type.SmoothStep
            );

            RecreateNavigation(1);
        }

        public IEnumerator SwitchPageAnimation(RectTransform fromPage, RectTransform toPage, int num)
        {
            switchingPages = true;
            Vector2 fromStartPosition = Vector2.zero, fromEndPosition = Vector2.zero;
            Vector2 toStartPosition = Vector2.zero;
            // Going backwards
            if(page > num)
            {
                fromStartPosition = new Vector2(pageXPosition, 0f);
                fromEndPosition = offScreenPagePosition;
                toStartPosition = new Vector2(-(fromPage.sizeDelta.x *4), 0f);
            }
            else
            {
                fromStartPosition = new Vector2(pageXPosition, 0f);
                fromEndPosition = new Vector2(-(fromPage.sizeDelta.x*4), 0f);
                toStartPosition = offScreenPagePosition;
            }

            StartCoroutine(LerpHelper.QuickTween(
                (v) => fromPage.anchoredPosition = v,
                fromStartPosition, fromEndPosition, pageMoveOutTime,
                lerpType: LerpHelper.Type.EaseIn
            ));

            toPage.gameObject.SetActive(true);
            yield return LerpHelper.QuickTween(
                (v) => toPage.anchoredPosition = v,
                toStartPosition, new Vector2(pageXPosition, 0f), pageMoveInTime,
                lerpType: LerpHelper.Type.EaseOut
            );
            fromPage.gameObject.SetActive(false);

            page = num;
            switchingPages = false;
            RecreateNavigation(page);
        }

        private void RandomizeAll()
        {
            foreach(var field in page1RandomisableFields)
                field.Randomise();
            foreach(var field in page2RandomisableFields)
                field.Randomise();
            foreach(var field in page3RandomisableFields)
                field.Randomise();
        }

        private void RandomizeButton()
        {
            AppearenceField[] fields;
            if(page == 1)
                fields = page1RandomisableFields;
            else if(page == 2)
                fields = page2RandomisableFields;
            else if(page == 3)
                fields = page3RandomisableFields;
            else
                return;
            foreach(var field in fields)
                field.Randomise();
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
            if(switchingPages)
                return;
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
            if(switchingPages)
                return;
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
            if(switchingPages)
                return;

            // End navigation
            foreach(var navigator in fieldNavigators)
                Destroy(navigator);
            fieldNavigators = new List<GUINavigator>();

            // Save character settings to state
            var state = GameState.GetInstance();
            state.Clear();
            state.Store(character);

            // Wave at me little guy
            StartCoroutine(FinishAnimation());
        }

        private IEnumerator FinishAnimation()
        {
            character.mainAnimator.SetBool("DoWave", true);
            yield return new WaitForSeconds(1f);
            character.mainAnimator.SetBool("DoWave", false);
            yield return StartCoroutine(
                screenFade.FadeOut(callback: () => FindObjectOfType<App>().StartNewState(AppState.Game)));
        }

        // --------------------------------------------------
        // GUI Navigation
        // --------------------------------------------------

        private void RecreateNavigation(int pageNum)
        {
            // Destroy existing navigators
            foreach(var navigator in fieldNavigators)
                Destroy(navigator);
            fieldNavigators = new List<GUINavigator>();

            // Get list of target elements
            MultiDimensionalRectTransform[] targetList;
            switch(pageNum)
            {
                case 1:
                    targetList = page1NavigationTargets;
                    break;
                case 2:
                    targetList = page2NavigationTargets;
                    break;
                case 3:
                    targetList = page3NavigationTargets;
                    break;
                default:
                    targetList = new MultiDimensionalRectTransform[0];
                    break;
            }

            // Go through each row of taregts, create Navigator for each one
            GUINavigator previousNavigator = null;
            foreach(var targetRow in targetList)
            {
                var fieldNavigator = gameObject.AddComponent<GUINavigator>();
                fieldNavigator.direction = GUINavigator.GUINavigatorDirection.Horizontal;
                fieldNavigator.oppositeAxisLinking = true;
                fieldNavigator.active = false;
                fieldNavigator.autoFlowFromLinkedNavigator = false;

                // Handle row linking
                if(previousNavigator != null)
                {
                    fieldNavigator.previousLinkedNavigator = previousNavigator;
                    previousNavigator.nextLinkedNavigator = fieldNavigator;
                }

                // Add individual elements to field
                foreach(var targetElement in targetRow.rectTransformArray)
                {
                    // Special fields
                    var colourField = targetElement.GetComponent<AppearenceFieldColour>();
                    var opacityField = targetElement.GetComponent<AppearenceFieldOpacity>();

                    // Add all swatches for colour fields
                    if(colourField != null)
                    {
                        foreach(var swatch in colourField.swatchHolder.GetComponentsInChildren<GlobalSelectableButton>())
                            fieldNavigator.AddNavigationElement(swatch.rectTransform);
                        fieldNavigator.AddNavigationElement(colourField.paletteButton.rectTransform);
                        colourField.navigator = fieldNavigator;
                    }
                    // Tell the opacity field which navigatior they came frome
                    else if(opacityField != null)
                    {
                        opacityField.navigator = fieldNavigator;
                        fieldNavigator.AddNavigationElement(opacityField.slider.GetComponent<RectTransform>());
                        fieldNavigator.AddNavigationElement(opacityField.flipButton.rectTransform);
                    }
                    else
                    {
                        fieldNavigator.AddNavigationElement(targetElement);
                    }
                }

                // Add to list for deletion when page changes
                fieldNavigators.Add(fieldNavigator);
                previousNavigator = fieldNavigator;
            }

            // Activate second one, skipping randomise button etc
            if(fieldNavigators.Count > 1)
                fieldNavigators[1].active = true;
        }

    }
}