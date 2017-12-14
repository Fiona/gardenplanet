using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace StompyBlondie
{
	/*
	 * Designed to be created with the ShowDialoguePopup static.
	 */
	public class DialoguePopup: BasePopup
	{
		[Header("Object references")]
		public GameObject source;
		public GameObject sourceNoPortrait;
		public TextMeshProUGUI dialogueText;
		public Image portrait;

		/*
		 * Creates the dialogue popup, is a coroutine that only finishes once the player has dismissed it.
		 * source - name of the character saying it.
		 * text - the text to display.
		 * sourcePortrait - An optional portrait image to display with the text.
		 */
		public static IEnumerator ShowDialoguePopup(string source, string text, Sprite portrait = null)
		{
			var popupObject = BasePopup.InitPopup<DialoguePopup>("prefabs/gui/DialoguePopup");
			popupObject.SetAttributes(source, text, portrait);
			yield return popupObject.StartCoroutine(popupObject.DoPopup());
		}

		public void SetAttributes(string sourceName, string text, Sprite portraitSprite)
		{
			source.GetComponentInChildren<TextMeshProUGUI>().text = sourceName;
			sourceNoPortrait.GetComponentInChildren<TextMeshProUGUI>().text = sourceName;
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
	}
}

