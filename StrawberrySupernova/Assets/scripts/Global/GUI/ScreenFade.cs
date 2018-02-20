using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace StompyBlondie
{
	/*
	 * Do FadeOut, FadeIn or the more controllable Fade to do a screen fade.
	 * Time in seconds, colours from and to and callback can be passed
	 * and are all optional.
	 * By default the fade is black and takes a second.
	 */
    public class ScreenFade: MonoBehaviour
	{

		Color defaultColour = new Color(0f, 0f, 0f);

        public IEnumerator FadeOut(float timeToFade = 1f, Color? fadeColour = null, Action callback = null)
        {
            Color? colorFrom, colorTo;
            if(!fadeColour.HasValue)
            {
                colorFrom = new Color(defaultColour.r, defaultColour.g, defaultColour.b, 0f);
                colorTo = new Color(defaultColour.r, defaultColour.g, defaultColour.b, 1f);
            }
            else
            {
                colorFrom = new Color(fadeColour.Value.r, fadeColour.Value.g, fadeColour.Value.b, 0f);
                colorTo = new Color(fadeColour.Value.r, fadeColour.Value.g, fadeColour.Value.b, 1f);
            }
            yield return StartCoroutine(Fade(timeToFade, colorFrom, colorTo, callback));
        }

        public IEnumerator FadeIn(float timeToFade = 1f, Color? fadeColour = null, Action callback = null)
        {
            Color? colorFrom, colorTo;
            if(!fadeColour.HasValue)
            {
                colorFrom = new Color(defaultColour.r, defaultColour.g, defaultColour.b, 1f);
                colorTo = new Color(defaultColour.r, defaultColour.g, defaultColour.b, 0f);
            }
            else
            {
                colorFrom = new Color(fadeColour.Value.r, fadeColour.Value.g, fadeColour.Value.b, 1f);
                colorTo = new Color(fadeColour.Value.r, fadeColour.Value.g, fadeColour.Value.b, 0f);
            }
            yield return StartCoroutine(Fade(timeToFade, colorFrom, colorTo, callback));
        }

		public IEnumerator Fade(float timeToFade = 1f, Color? colourFrom = null, Color? colourTo = null, Action callback = null)
		{
			if(!colourTo.HasValue)
				colourTo = defaultColour;
            if(!colourFrom.HasValue)
				colourFrom = defaultColour;

            var img = GetComponent<Image>();
            var t = 0f;

			img.raycastTarget = true;
            while(t < 1)
            {
                t += Time.deltaTime / timeToFade;
                img.color = Color.Lerp((Color)colourFrom, (Color)colourTo, t);
                yield return new WaitForFixedUpdate();
            }
			img.raycastTarget = false;

			if(callback != null)
				callback();
		}

	}
}

