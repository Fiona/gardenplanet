using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StompyBlondie
{
    public class LerpHelper
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

