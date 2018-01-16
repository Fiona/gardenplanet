using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace StompyBlondie
{
    public static class LerpHelper
    {

        public enum Type
        {
            SmoothStep,
            EaseIn,
            EaseOut,
            Exponential,

            BounceIn,
            BounceOut,
            BounceInOut,

            Linear
        }

        private static float BounceIn(float t)
        {
            return 1f - BounceOut(1f - t);
        }

        private static float BounceOut(float t)
        {
            if(t < (1f/2.75f))
                return 7.5625f*t*t;
            if(t < (2f/2.75f))
                return 7.5625f*(t -= (1.5f/2.75f))*t + 0.75f;
            if(t < (2.5f/2.75f))
                return 7.5625f *(t -= (2.25f/2.75f))*t + 0.9375f;
            return 7.5625f*(t -= (2.625f/2.75f))*t + 0.984375f;
        }


        public static float ApplyLerpType(float t, LerpHelper.Type type)
        {
            switch(type)
            {
                case LerpHelper.Type.SmoothStep:
                    return t * t * (3f - 2f * t);
                case LerpHelper.Type.EaseIn:
                    return 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
                case LerpHelper.Type.EaseOut:
                    return Mathf.Sin(t * Mathf.PI * 0.5f);
                case LerpHelper.Type.Exponential:
                    return t * t;

                case LerpHelper.Type.BounceIn:
                    return BounceIn(t);
                case LerpHelper.Type.BounceOut:
                    return BounceOut(t);
                case LerpHelper.Type.BounceInOut:
                    if(t < 0.5f)
                        return BounceIn(t*2f)*0.5f;
                    return BounceOut(t*2f - 1f)*0.5f + 0.5f;

                default:
                    return t;
            }
        }

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
        public static IEnumerator QuickTween(Action<float> callback, float fromValue, float toValue,
            float durationSeconds, Action onDone = null, LerpHelper.Type lerpType = LerpHelper.Type.Linear)
        {
            foreach(var val in LerpHelper.LerpOverTime(durationSeconds))
            {
                callback(Mathf.Lerp(fromValue, toValue, ApplyLerpType(val, lerpType)));
                yield return new WaitForFixedUpdate();
            }

            if(onDone != null)
                onDone();

            yield break;
        }

        /*
         * Returns a coroutine that does a quick tween a property between two values. Pass to StartCoroutine.
         */
        public static IEnumerator QuickTween(Action<Vector3> callback, Vector3 fromValue, Vector3 toValue,
            float durationSeconds, Action onDone = null, LerpHelper.Type lerpType = LerpHelper.Type.Linear)
        {
            foreach(var val in LerpHelper.LerpOverTime(durationSeconds))
            {
                callback(Vector3.Lerp(fromValue, toValue, ApplyLerpType(val, lerpType)));
                yield return new WaitForFixedUpdate();
            }

            if(onDone != null)
                onDone();

            yield break;
        }

        /*
        * Returns a coroutine that does a quick tween a property between two values. Pass to StartCoroutine.
        */
        public static IEnumerator QuickTween(Action<Color> callback, Color fromValue, Color toValue,
            float durationSeconds, Action onDone = null, LerpHelper.Type lerpType = LerpHelper.Type.Linear)
        {
            foreach(var val in LerpHelper.LerpOverTime(durationSeconds))
            {
                callback(Color.Lerp(fromValue, toValue, ApplyLerpType(val, lerpType)));
                yield return new WaitForFixedUpdate();
            }

            if(onDone != null)
                onDone();

            yield break;
        }

        /*
        * Commonly used tween to fade from 0 to 1, using a canvas group
        */
        public static IEnumerator QuickFadeIn(CanvasGroup canvasGroup, float durationSeconds = 1f,
            LerpHelper.Type lerpType = LerpHelper.Type.Linear)
        {
            yield return LerpHelper.QuickTween(
                (i) => { canvasGroup.alpha = i; },
                0f, 1f, durationSeconds, lerpType: lerpType
            );
        }

        /*
        * Commonly used tween to fade from 0 to 1, using an image
        */
        public static IEnumerator QuickFadeIn(Image image, float durationSeconds = 1f,
            LerpHelper.Type lerpType = LerpHelper.Type.Linear)
        {
            yield return LerpHelper.QuickTween(
                (i) => { image.color = new Color(image.color.r, image.color.g, image.color.b, i); },
                0f, 1f, durationSeconds, lerpType: lerpType
            );
        }

        /*
        * Commonly used tween to fade from 1 to 0, using a canvas group
        */
        public static IEnumerator QuickFadeOut(CanvasGroup canvasGroup, float durationSeconds = 1f,
            LerpHelper.Type lerpType = LerpHelper.Type.Linear)
        {
            yield return LerpHelper.QuickTween(
                (i) => { canvasGroup.alpha = i; },
                1f, 0f, durationSeconds, lerpType: lerpType
            );
        }

        /*
        * Commonly used tween to fade from 1 to 0, using an image
        */
        public static IEnumerator QuickFadeOut(Image image, float durationSeconds = 1f,
            LerpHelper.Type lerpType = LerpHelper.Type.Linear)
        {
            yield return LerpHelper.QuickTween(
                (i) => { image.color = new Color(image.color.r, image.color.g, image.color.b, i); },
                1f, 0f, durationSeconds, lerpType: lerpType
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