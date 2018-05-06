using System.Collections;
using Global.Core;
using UnityEngine;

namespace StompyBlondie
{
    public static class AudioSourceHelper
    {
        public static void FadeOut(this AudioSource source, float fadeOutTime = 1f)
        {
            ExtensionMonoBehaviour.GetInstance().StartCoroutine(DoFadeOut(source, fadeOutTime));
        }

        private static IEnumerator DoFadeOut(AudioSource source, float fadeOutTime)
        {
            yield return LerpHelper.QuickTween(
                (v) => source.volume = v, source.volume, 0f, fadeOutTime
                );
            source.Stop();
        }
    }
}