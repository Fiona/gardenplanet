using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace StompyBlondie
{
	/*
	 * Designed to be created with the ShowDialoguePopup static.
	 */
	public class DialoguePopup: MonoBehaviour
	{
		const string prefabName = "prefabs/gui/DialoguePopup";

		[Header("Object references")]
		public GameObject source;
		public GameObject sourceNoPortrait;
		public Text dialogueText;
		public Image portrait;

		[HideInInspector]
		public bool closePopup;

		/*
		 * Creates the dialogue popup, is a coroutine that only finishes once the player has dismissed it.
		 * source - name of the character saying it.
		 * text - the text to display.
		 * sourcePortrait - An optional portrait image to display with the text.
		 */
		public static IEnumerator ShowDialoguePopup(string source, string text, Sprite portrait = null)
		{
			// Create dialogue
			var resource = Resources.Load(DialoguePopup.prefabName) as GameObject;
			var dialoguePopupObject = Instantiate(resource);

			// Add to canvas
			var canvas = FindObjectOfType<Canvas>();
			if(canvas == null)
				throw new Exception("Can't find a Canvas to attach to.");

			var comp = dialoguePopupObject.GetComponent<DialoguePopup>();

			dialoguePopupObject.transform.SetParent(canvas.transform, false);

			// Set attributes 
			comp.SetAttributes(source, text, portrait);

			// Wait for input
			comp.closePopup = false;
			while(!comp.closePopup)
				yield return new WaitForFixedUpdate();

			// Remove from canvas
			Destroy(dialoguePopupObject);
		
		}

		public void SetAttributes(string sourceName, string text, Sprite portraitSprite)
		{
			source.GetComponentInChildren<Text>().text = sourceName;
			sourceNoPortrait.GetComponentInChildren<Text>().text = sourceName;
			dialogueText.text = text;
			if(portraitSprite == null)
			{
				source.SetActive(false);
				sourceNoPortrait.SetActive(true);
			}
			else
			{
				source.SetActive(false);
				sourceNoPortrait.SetActive(false);
				portrait.overrideSprite = portraitSprite;
				portrait.rectTransform.sizeDelta = new Vector2(portraitSprite.texture.width, portraitSprite.texture.height);
			}
		}

		public void Update()
		{
			if(closePopup)
				return;
			if(Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return))
				closePopup = true;
		}

		public void ClickedOnPopup()
		{
			closePopup = true;
		}
	}
}

