using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using StrawberryNova;
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
		public GameObject positionMarkers;
		public Image bobber;

		/*
		 * Creates the message popup, is a coroutine that only finishes once the player has dismissed it.
		 * text - the text to display.
		 */
		public static IEnumerator ShowMessagePopup(string text)
		{
			var popupObject = BasePopup.InitPopup<MessagePopup>("prefabs/gui/MessagePopup");
			popupObject.messageText.text = text;
			popupObject.SetPosition(PopupPositions.Centre);
			yield return popupObject.StartCoroutine(popupObject.DoPopup());
		}

		public override IEnumerator AnimOpen()
		{
			bobber.color = new Color(1f, 1f, 1f, 0f);
			yield return LerpHelper.QuickTween(
				(v) => { popupObject.transform.localScale = v; },
				Vector3.zero,
				Vector3.one,
				.4f,
				lerpType:LerpHelper.Type.EaseOut
			);
			yield return LerpHelper.QuickFadeIn(bobber, .2f, lerpType:LerpHelper.Type.SmoothStep);
		}

		public override IEnumerator AnimClose()
		{
			yield return LerpHelper.QuickFadeOut(bobber, .1f, lerpType:LerpHelper.Type.SmoothStep);
			yield return LerpHelper.QuickTween(
				(v) => { popupObject.transform.localScale = v; },
				Vector3.one,
				Vector3.zero,
				.25f,
				lerpType:LerpHelper.Type.EaseIn
			);
		}

		private void SetPosition(PopupPositions position)
		{
			var marker = positionMarkers.transform.Find(position.ToString());
			popupObject.position = marker.position;
		}

	}
}