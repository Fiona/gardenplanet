using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace StompyBlondie
{
	/*
	 * Designed to be created with the ShowChoicePopup static.
	 */
	public class ChoicePopup: MonoBehaviour
	{

		public Text questionText;
		public Button choiceTemplate;
		public GameObject choicesHolder;
		[HideInInspector]
		public Ref<int> selectedChoice;

		const string prefabName = "prefabs/gui/ChoicePopup";
		bool closePopup;

		/*
		 * Creates the choice popup, is a coroutine that only finishes once the player has selected
		 * an option. Pass in a Ref object holding an int and read it back after for determining which one
		 * they chose.
		 */
		public static IEnumerator ShowChoicePopup(string question, string[] choices, Ref<int> result)
		{

			// Create pop up
			var resource = Resources.Load(ChoicePopup.prefabName) as GameObject;
			var choicePopupObject = Instantiate(resource);

			// Add to canvas
			var canvas = FindObjectOfType<Canvas>();
			if(canvas == null)
				throw new Exception("Can't find a Canvas to attach to.");

			var comp = choicePopupObject.GetComponent<ChoicePopup>();

			choicePopupObject.transform.SetParent(canvas.transform, false);
			comp.selectedChoice = result;

			// Replace question text
			comp.SetQuestionText(question);

			// Set choices
			comp.choiceTemplate.gameObject.SetActive(false);
			var i = 1;
			foreach(var choice in choices)
			{
				comp.AddChoice(choice, i);
				i++;
			}

			// Wait
			comp.selectedChoice.Value = -1;

			while(!comp.closePopup)
				yield return new WaitForFixedUpdate();	

			Destroy(choicePopupObject);
			
		}

		void SetQuestionText(string question)
		{
			questionText.text = question;
		}

		void AddChoice(string choice, int num)
		{
			var newChoice = Instantiate(choiceTemplate);
			newChoice.gameObject.GetComponentInChildren<Text>().text = choice;
			newChoice.transform.SetParent(choicesHolder.transform, false);
			newChoice.gameObject.SetActive(true);
			newChoice.onClick.AddListener(
				delegate
				{
					ChoiceSelected(num);
				}
			);
		}

		void ChoiceSelected(int num)
		{
			selectedChoice.Value = num;
			closePopup = true;
		}

	}
}

