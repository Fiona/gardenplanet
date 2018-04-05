using System;
using System.Collections;
using NUnit.Framework;
using Rewired;
using StompyBlondie;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UI.Extensions.ColorPicker;
using UnityEngine.UI.Extensions;

namespace StrawberryNova
{
    public class CACColourPicker: MonoBehaviour
    {
        public float fadeTime = .5f;
        public float inputChangeValueSpeed = .01f;

        private EventTrigger eventTrigger;
        private CanvasGroup canvasGroup;
        private ColorPickerControl colorPickerControl;
        private Action<Color> callback;
        private GUINavigator navigator;
        private GameInputManager input;
        private BoxSlider colourSVSlider;
        private Slider colourHSlider;
        private string inputBehaviourXML;
        private CharacterRotationPanel rotationPanel;

        public static void ShowColourPicker(GlobalButton buttonPressed, Color startColor, Action<Color> callback,
            GUINavigator _navigator)
        {
            var picker = FindObjectOfType<CACColourPicker>();
            picker.Show(buttonPressed, startColor, callback, _navigator);
        }

        public void Hide(bool firstRun = false)
        {
            if(canvasGroup.alpha >= 1f)
                StartCoroutine(DoHide(firstRun));
        }

        public void Show(GlobalButton buttonPressed, Color startColor, Action<Color> _callback, GUINavigator _navigator)
        {
            navigator = _navigator;
            if(navigator != null)
                navigator.active = false;
            var controlTransform = colorPickerControl.gameObject.GetComponent<RectTransform>();
            controlTransform.position = buttonPressed.rectTransform.position;
            colorPickerControl.CurrentColor = startColor;
            callback = _callback;
            StartCoroutine(DoShow());
        }

        private void Start()
        {
            rotationPanel = FindObjectOfType<CharacterRotationPanel>();
            input = FindObjectOfType<GameInputManager>();
            canvasGroup = GetComponent<CanvasGroup>();
            colorPickerControl = GetComponentInChildren<ColorPickerControl>();
            colourSVSlider = colorPickerControl.GetComponentInChildren<BoxSlider>(true);
            colourHSlider = colorPickerControl.GetComponentInChildren<ColorSlider>(true).GetComponent<Slider>();

            eventTrigger = gameObject.AddComponent<EventTrigger>();
            var clickEntry = new EventTrigger.Entry();
            clickEntry.eventID = EventTriggerType.PointerClick;
            clickEntry.callback.AddListener(ClickedBehind);
            eventTrigger.triggers.Add(clickEntry);

            colorPickerControl.onValueChanged.AddListener(ColorPickerChanged);

            Hide(true);
        }

        public void Update()
        {
            if(canvasGroup.alpha < 1f)
                return;

            if(input.player.GetButtonRepeating("Menu Left"))
                colourSVSlider.ValueX -= inputChangeValueSpeed;
            if(input.player.GetButtonRepeating("Menu Right"))
                colourSVSlider.ValueX += inputChangeValueSpeed;
            if(input.player.GetButtonRepeating("Menu Up"))
                colourSVSlider.ValueY += inputChangeValueSpeed;
            if(input.player.GetButtonRepeating("Menu Down"))
                colourSVSlider.ValueY -= inputChangeValueSpeed;

            if(input.player.GetButtonRepeating("Previous Page"))
                colourHSlider.value += inputChangeValueSpeed/2;
            if(input.player.GetButtonRepeating("Next Page"))
                colourHSlider.value -= inputChangeValueSpeed/2;

            if(input.player.GetButtonUp("Confirm") || input.player.GetButtonUp("Cancel"))
                Hide();
        }

        public void ClickedBehind(BaseEventData data)
        {
            Hide();
        }

        private IEnumerator DoShow()
        {
            canvasGroup.blocksRaycasts = true;
            yield return LerpHelper.QuickFadeIn(canvasGroup, fadeTime);

            rotationPanel.canRotate = false;

            // Backup the menu direction action behaviour to restore later.
            // The menu direction behaviour causes some undesired stuttering when using menu actions with
            // the colour picker, so we will change the behaviour to the default one and restore it after we close
            // the colour picker in DoHide.
            var defaultBehaviour = ReInput.mapping.GetInputBehavior(
                Consts.REWIRED_PLAYER_ID, Consts.REWIRED_INPUT_BEHAVIOUR_DEFAULT
                );
            var menuDirectionsBehaviour = ReInput.mapping.GetInputBehavior(
                Consts.REWIRED_PLAYER_ID, Consts.REWIRED_INPUT_BEHAVIOUR_MENUDIRECTIONS
               );
            inputBehaviourXML = menuDirectionsBehaviour.ToXmlString();
            menuDirectionsBehaviour.ImportData(defaultBehaviour);
        }

        private IEnumerator DoHide(bool firstRun)
        {
            callback = null;

            if(firstRun)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
                yield break;
            }

            rotationPanel.canRotate = true;

            // Restore the action behaviour that we overwrote in DoShow
            var menuDirectionsBehaviour = ReInput.mapping.GetInputBehavior(
                Consts.REWIRED_PLAYER_ID, Consts.REWIRED_INPUT_BEHAVIOUR_MENUDIRECTIONS
            );
            menuDirectionsBehaviour.ImportXmlString(inputBehaviourXML);

            if(navigator != null)
                navigator.active = true;

            yield return LerpHelper.QuickFadeOut(canvasGroup, fadeTime);
            canvasGroup.blocksRaycasts = false;
        }

        private void ColorPickerChanged(Color newColor)
        {
            if(callback != null)
                callback(newColor);
        }

    }
}