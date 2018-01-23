using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using StrawberryNova;
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
		public TextMeshProUGUI dialogueText;
		public GameObject positionMarkers;

		private PopupPositions position;

		/*
		 * Creates the dialogue popup, is a coroutine that only finishes once the player has dismissed it.
		 * source - name of the character saying it.
		 * text - the text to display.
		 */
		public static IEnumerator ShowDialoguePopup(string source, string text)
		{
			var popupObject = BasePopup.InitPopup<DialoguePopup>("prefabs/gui/DialoguePopup");
			popupObject.SetAttributes(source, text);
			popupObject.SetPosition(PopupPositions.TopLeft);
			yield return popupObject.StartCoroutine(popupObject.DoPopup());
		}

		public override IEnumerator AnimOpen()
		{
			yield return LerpHelper.QuickTween(
				(v) => { popupObject.transform.localScale = v; },
				Vector3.zero,
				Vector3.one,
				.5f,
				lerpType:LerpHelper.Type.BounceOut
			);
		}

		public override IEnumerator AnimClose()
		{
			yield return LerpHelper.QuickTween(
				(v) => { popupObject.transform.localScale = v; },
				Vector3.one,
				Vector3.zero,
				.25f,
				lerpType:LerpHelper.Type.EaseOut
			);
		}

		private void SetAttributes(string sourceName, string text)
		{
			source.GetComponentInChildren<TextMeshProUGUI>().text = sourceName;
			dialogueText.text = text;
		}

		private void SetPosition(PopupPositions position)
		{
			this.position = position;
			var marker = positionMarkers.transform.Find(position.ToString());
			popupObject.position = marker.position;
		}

	}
}

