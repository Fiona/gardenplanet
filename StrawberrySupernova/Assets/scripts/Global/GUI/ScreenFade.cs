using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace StompyBlondie
{
	/*
	 * Do ScreenFade.FadeOut or ScreenFade.FadeIn to do a screen fade.
	 * Time in seconds, colours from and to and callback can be passed
	 * and are all optional.
	 * By default the fade is black and takes a second.
	 */
	public class ScreenFade
	{

		const string prefabName = "prefabs/gui/ScreenFade";
		static Color defaultColour = new Color(0f, 0f, 0f);

		GameObject screenFadeObject;

		public static IEnumerator FadeOut(float timeToFade = 1f, Color? colourFrom = null, Color? colourTo = null, Action callback = null)
		{
			if(!colourTo.HasValue)
				colourTo = defaultColour;
			if(colourFrom.HasValue)
				colourFrom = defaultColour;
			colourTo = new Color(colourTo.Value.r, colourTo.Value.g, colourTo.Value.b, 1f);
			colourFrom = new Color(colourTo.Value.r, colourTo.Value.g, colourTo.Value.b, 0f);

			var fadeObj = new ScreenFade();
			yield return fadeObj.screenFadeObject.GetComponent<Image>().StartCoroutine(fadeObj.DoFade((Color)colourFrom, (Color)colourTo, timeToFade));
			if(callback != null)
				callback();
			UnityEngine.Object.Destroy(fadeObj.screenFadeObject);
		}

		public static IEnumerator FadeIn(float timeToFade = 1f, Color? colourFrom = null, Color? colourTo = null, Action callback = null)
		{
			if(!colourTo.HasValue)
				colourTo = defaultColour;
			if(!colourFrom.HasValue)
				colourFrom = defaultColour;
			colourTo = new Color(colourTo.Value.r, colourTo.Value.g, colourTo.Value.b, 0f);
			colourFrom = new Color(colourTo.Value.r, colourTo.Value.g, colourTo.Value.b, 1f);

			var fadeObj = new ScreenFade();
			yield return fadeObj.screenFadeObject.GetComponent<Image>().StartCoroutine(fadeObj.DoFade((Color)colourFrom, (Color)colourTo, timeToFade));
			if(callback != null)
				callback();
			UnityEngine.Object.Destroy(fadeObj.screenFadeObject);
		}
			
		public ScreenFade()
		{
			var resource = Resources.Load(ScreenFade.prefabName) as GameObject;
			screenFadeObject = UnityEngine.Object.Instantiate(resource);

			var canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
			if(canvas == null)
				throw new Exception("Can't find a Canvas to attach to.");

			screenFadeObject.transform.SetParent(canvas.transform, false);
		}

		public IEnumerator DoFade(Color colourFrom, Color colourTo, float timeToFade)
		{
			var img = screenFadeObject.GetComponent<Image>();
			var t = 0f;
			while(t < 1)
			{
				t += Time.deltaTime / timeToFade;
				img.color = Color.Lerp(colourFrom, colourTo, t);
				yield return new WaitForFixedUpdate();
			}
		}
			
	}
}

