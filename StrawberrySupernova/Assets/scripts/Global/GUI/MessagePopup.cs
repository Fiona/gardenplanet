using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace StompyBlondie
{
	/*
	 * Designed to be created with the ShowMessagePopup static.
	 */
	public class MessagePopup: BasePopup
	{
		[Header("Object references")]
		public TextMeshProUGUI messageText;

		/*
		 * Creates the message popup, is a coroutine that only finishes once the player has dismissed it.
		 * text - the text to display.
		 */
		public static IEnumerator ShowMessagePopup(string text)
		{
			var popupObject = BasePopup.InitPopup<MessagePopup>("prefabs/gui/MessagePopup");
			popupObject.messageText.text = text;
			yield return popupObject.StartCoroutine(popupObject.DoPopup());
		}
	}
}