using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
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
		public Button choiceTemplate;
		public GameObject choicesHolder;
		[HideInInspector]
		public Ref<int> selectedChoice;

		/*
		 * Creates the choice popup, is a coroutine that only finishes once the player has selected
		 * an option. Pass in a Ref object holding an int and read it back after for determining which one
		 * they chose, starting at 1.
		 */
		public static IEnumerator ShowChoicePopup(string question, string[] choices, Ref<int> result)
		{
			var popupObject = BasePopup.InitPopup<ChoicePopup>("prefabs/gui/ChoicePopup");
			popupObject.SetAttributes(question, choices, result);
			yield return popupObject.StartCoroutine(popupObject.DoPopup());
		}

		public void SetAttributes(string question, string[] choices, Ref<int> result)
		{
			questionText.text = question;

			choiceTemplate.gameObject.SetActive(false);
			var i = 1;
			foreach(var choice in choices)
			{
				AddChoice(choice, i);
				i++;
			}

			selectedChoice = result;
			selectedChoice.Value = -1;
		}

		void AddChoice(string choice, int num)
		{
			var newChoice = Instantiate(choiceTemplate);
			newChoice.gameObject.GetComponentInChildren<TextMeshProUGUI>().text = choice;
			newChoice.transform.SetParent(choicesHolder.transform, false);
			newChoice.gameObject.SetActive(true);
			newChoice.onClick.AddListener(
				delegate
				{
					ChoiceSelected(num);
				}
			);
		}

		public override void ClickedOnPopup()
		{
		}

		void ChoiceSelected(int num)
		{
			selectedChoice.Value = num;
			closePopup = true;
		}

	}
}

