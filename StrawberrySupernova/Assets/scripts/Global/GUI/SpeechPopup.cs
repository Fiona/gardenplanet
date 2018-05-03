using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Security.Cryptography;
using GardenPlanet;
using TMPro;

namespace StompyBlondie
{
	/*
	 * Designed to be created with the ShowSpeechPopup static.
	 */
	public class SpeechPopup: BasePopup
	{
		[Header("Object references")]
		public GameObject source;
		public TextMeshProUGUI sourceText;
		public TextMeshProUGUI speechText;
		public GameObject positionMarkers;
		public Image bobber;

		/*
		 * Creates the dialogue popup, is a coroutine that only finishes once the player has dismissed it.
		 * source - name of the character saying it.
		 * text - the text to display.
		 */
		public static IEnumerator ShowSpeechPopup(string source, string text)
		{
			var popupObject = BasePopup.InitPopup<SpeechPopup>("prefabs/gui/SpeechPopup");
			popupObject.SetAttributes(source, text);
			popupObject.SetPosition(PopupPositions.TopLeft);
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
				lerpType:LerpHelper.Type.BounceOut
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
				lerpType:LerpHelper.Type.EaseOut
			);
		}

		private void SetAttributes(string sourceName, string text)
		{
			sourceText.text = sourceName;
			speechText.text = text;
		}

		private void SetPosition(PopupPositions position)
		{
			var marker = positionMarkers.transform.Find(position.ToString());
			popupObject.position = marker.position;
		}

	}
}

