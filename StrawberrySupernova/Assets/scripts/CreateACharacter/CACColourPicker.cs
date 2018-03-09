using System;
using System.Collections;
using StompyBlondie;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI.Extensions.ColorPicker;

namespace StrawberryNova
{
    public class CACColourPicker: MonoBehaviour
    {
        public float fadeTime = .5f;

        private EventTrigger eventTrigger;
        private CanvasGroup canvasGroup;
        private ColorPickerControl colorPickerControl;
        private Action<Color> callback;

        public static void ShowColourPicker(GlobalButton buttonPressed, Color startColor, Action<Color> callback)
        {
            var picker = FindObjectOfType<CACColourPicker>();
            picker.Show(buttonPressed, startColor, callback);
        }

        public void Hide(bool immediate = false)
        {
            if(canvasGroup.alpha >= 1f)
                StartCoroutine(DoHide(immediate));
        }

        public void Show(GlobalButton buttonPressed, Color startColor, Action<Color> _callback)
        {
            var controlTransform = colorPickerControl.gameObject.GetComponent<RectTransform>();
            controlTransform.position = buttonPressed.rectTransform.position;
            colorPickerControl.CurrentColor = startColor;
            callback = _callback;
            StartCoroutine(DoShow());
        }

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            colorPickerControl = GetComponentInChildren<ColorPickerControl>();

            eventTrigger = gameObject.AddComponent<EventTrigger>();
            var clickEntry = new EventTrigger.Entry();
            clickEntry.eventID = EventTriggerType.PointerClick;
            clickEntry.callback.AddListener(ClickedBehind);
            eventTrigger.triggers.Add(clickEntry);

            colorPickerControl.onValueChanged.AddListener(ColorPickerChanged);

            Hide(true);
        }

        public void ClickedBehind(BaseEventData data)
        {
            Hide();
        }

        private IEnumerator DoHide(bool immediate)
        {
            callback = null;

            if(immediate)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.blocksRaycasts = false;
                yield break;
            }

            yield return LerpHelper.QuickFadeOut(canvasGroup, fadeTime);
            canvasGroup.blocksRaycasts = false;
        }

        private IEnumerator DoShow()
        {
            canvasGroup.blocksRaycasts = true;
            yield return LerpHelper.QuickFadeIn(canvasGroup, fadeTime);
        }

        private void ColorPickerChanged(Color newColor)
        {
            if(callback != null)
                callback(newColor);
        }

    }
}