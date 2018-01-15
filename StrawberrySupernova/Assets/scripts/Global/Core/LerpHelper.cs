using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StompyBlondie
{
    public static class LerpHelper
    {

        /*
         * Returns an enumerable that is used to allow easy lerping over a particular length of time.
         * The enumerable provides numbers between 0 and 1 as it is iterated over.
         * The desired duration in seconds is passed as a float.
         *
         * Example:
         *
         * foreach(var val in LerpHelper.LerpOverTime(5f)
         * {
         *     myAttr = Mathf.Lerp(0f, 10f, val);
         *     yield return new WaitForFixedUpdate();
         * }
         */
        public static IEnumerable<float> LerpOverTime(float durationSeconds)
        {
            return new TimeLerper(durationSeconds);
        }

        /*
         * Returns a coroutine that does a quick tween a property between two values. Pass to StartCoroutine.
         */
        public static IEnumerator QuickTween(Action<float> callback, float fromValue, float toValue, float durationSeconds, Action onDone = null)
        {
            foreach(var val in LerpHelper.LerpOverTime(durationSeconds))
            {
                callback(Mathf.Lerp(fromValue, toValue, val));
                yield return new WaitForFixedUpdate();
            }

            if(onDone != null)
                onDone();
        }

        /*
         * Returns a coroutine that does a quick tween a property between two values. Pass to StartCoroutine.
         */
        public static IEnumerator QuickTween(Action<Vector3> callback, Vector3 fromValue, Vector3 toValue, float durationSeconds, Action onDone = null)
        {
            foreach(var val in LerpHelper.LerpOverTime(durationSeconds))
            {
                callback(Vector3.Lerp(fromValue, toValue, val));
                yield return new WaitForFixedUpdate();
            }

            if(onDone != null)
                onDone();
        }

        /*
         * Returns a coroutine that does a quick tween a property between two values. Pass to StartCoroutine.
         */
        public static IEnumerator QuickTween(Action<Color> callback, Color fromValue, Color toValue, float durationSeconds, Action onDone = null)
        {
            foreach(var val in LerpHelper.LerpOverTime(durationSeconds))
            {
                callback(Color.Lerp(fromValue, toValue, val));
                yield return new WaitForFixedUpdate();
            }

            if(onDone != null)
                onDone();
        }

        /*
         * Commonly used tween to fade from 0 to 1, using a canvas group
         */
        public static IEnumerator QuickFadeIn(CanvasGroup canvasGroup, float durationSeconds = 1f)
        {
            yield return LerpHelper.QuickTween(
                (i) => { canvasGroup.alpha = i; },
                0f, 1f, durationSeconds
            );
        }

        /*
         * Commonly used tween to fade from 0 to 1, using an image
         */
        public static IEnumerator QuickFadeIn(Image image, float durationSeconds = 1f)
        {
            yield return LerpHelper.QuickTween(
                (i) => { image.color = new Color(image.color.r, image.color.g, image.color.b, i); },
                0f, 1f, durationSeconds
            );
        }

        /*
         * Commonly used tween to fade from 1 to 0, using a canvas group
         */
        public static IEnumerator QuickFadeOut(CanvasGroup canvasGroup, float durationSeconds = 1f)
        {
            yield return LerpHelper.QuickTween(
                (i) => { canvasGroup.alpha = i; },
                1f, 0f, durationSeconds
            );
        }

        /*
         * Commonly used tween to fade from 1 to 0, using an image
         */
        public static IEnumerator QuickFadeOut(Image image, float durationSeconds = 1f)
        {
            yield return LerpHelper.QuickTween(
                (i) => { image.color = new Color(image.color.r, image.color.g, image.color.b, i); },
                1f, 0f, durationSeconds
            );
        }

    }

    public class TimeLerper : IEnumerable<float>
    {
        float durationSeconds;
        public TimeLerper(float durationSeconds)
        {
            this.durationSeconds = durationSeconds;
        }

        public IEnumerator<float> GetEnumerator()
        {
            return new TimeLerp(durationSeconds);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class TimeLerp : IEnumerator<float>
    {
        public float Current
        {
            get
            {
                return currentTime;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return currentTime;
            }
        }

        float currentTime;
        float startTime;
        float durationSeconds;

        public TimeLerp(float durationSeconds)
        {
            this.durationSeconds = durationSeconds;
            startTime = Time.time;
            MoveNext();
        }

        public bool MoveNext()
        {
            if(currentTime >= 1f)
                return false;
            currentTime = Mathf.Clamp((Time.time-startTime) / durationSeconds, 0.0f, 1.0f);
            return true;
        }

        public void Reset()
        {
            startTime = Time.time;
        }

        public void Dispose()
        {

        }
    }
}