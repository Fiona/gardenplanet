using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using GardenPlanet;
using TMPro;

namespace StompyBlondie
{
    /*
     * Designed to be created with the ShowChoicePopup static.
     */
    public class ChoicePopup: BasePopup
    {
        [Header("Object References")]
        public TextMeshProUGUI questionText;
        public GameObject positionMarkers;
        public GlobalButton choiceTemplate;
        public GameObject choicesHolder;
        [HideInInspector]
        public Ref<int> selectedChoice;

        private List<GameObject> buttons;
        private GUINavigator navigator;

        /*
         * Creates the choice popup, is a coroutine that only finishes once the player has selected
         * an option. Pass in a Ref object holding an int and read it back after for determining which one
         * they chose, starting at 1.
         */
        public static IEnumerator ShowChoicePopup(string question, string[] choices, Ref<int> result, int cancelChoice = -1)
        {
            var popupObject = Construct(question, choices, result, cancelChoice);
            yield return popupObject.StartCoroutine(popupObject.DoPopup());
        }

        public static IEnumerator ShowYesNoPopup(string question, Ref<int> result)
        {
            var popupObject = Construct(question, new string[] {"Yes", "No"}, result, 1);
            yield return popupObject.StartCoroutine(popupObject.DoPopup());
        }

        public void SetAttributes(string question, string[] choices, Ref<int> result, int cancelChoice)
        {
            navigator = gameObject.AddComponent<GUINavigator>();
            buttons = new List<GameObject>();
            questionText.text = question;

            choiceTemplate.gameObject.SetActive(false);
            var i = 1;
            foreach(var choice in choices)
            {
                AddChoice(choice, i, cancelChoice == (choices.Length-1));
                i++;
            }

            selectedChoice = result;
            selectedChoice.Value = -1;
        }

        public override void ClickedOnPopup()
        {
        }

        public override IEnumerator AnimOpen()
        {
            yield return LerpHelper.QuickTween(
                (v) => { popupObject.transform.localScale = v; },
                Vector3.zero,
                Vector3.one,
                .4f,
                lerpType:LerpHelper.Type.EaseOut
            );
        }

        public override IEnumerator AnimClose()
        {
            yield return LerpHelper.QuickTween(
                (v) => { popupObject.transform.localScale = v; },
                Vector3.one,
                Vector3.zero,
                .25f,
                lerpType:LerpHelper.Type.EaseIn
            );
        }

        void AddChoice(string choice, int num, bool isCancel)
        {
            var newChoice = Instantiate(choiceTemplate);
            newChoice.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = choice;
            newChoice.transform.SetParent(choicesHolder.transform, false);
            newChoice.gameObject.SetActive(true);
            newChoice.SetCallback(() => ChoiceSelected(num));
            buttons.Add(newChoice.gameObject);
            navigator.AddNavigationElement(newChoice.GetComponent<RectTransform>(), isCancel);
        }

        public void ChoiceSelected(int num)
        {
            selectedChoice.Value = num;
            closePopup = true;
            navigator.HideNavPointer();
        }

        private static ChoicePopup Construct(string question, string[] choices, Ref<int> result, int cancelChoice)
        {
            var popupObject = BasePopup.InitPopup<ChoicePopup>("prefabs/gui/ChoicePopup");
            popupObject.SetAttributes(question, choices, result, cancelChoice);
            popupObject.SetPosition(PopupPositions.Centre);
            return popupObject;
        }

        private void SetPosition(PopupPositions position)
        {
            var marker = positionMarkers.transform.Find(position.ToString());
            popupObject.position = marker.position;
        }

    }
}