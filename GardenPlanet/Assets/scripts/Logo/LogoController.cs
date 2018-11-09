using System.Collections;
using System.Linq;
using Rewired;
using StompyBlondie;
using UnityEngine;
using UnityEngine.UI;
using StompyBlondie.Utils;
using StompyBlondie.Extensions;

namespace GardenPlanet
{
    public class LogoController : MonoBehaviour
    {
        [Header("Settings")]
        public Sprite[] characterSprites;
        public int numberOfStomps = 3;
        public float logoStartRotation = 20f;
        public float fadeInTime = 1f;
        public float fadeOutTime = 1f;
        public float beforeAnimWaitTime = 2f;
        public float footDownWaitTime = .5f;
        public float footLiftBetweenFramesTime = .1f;
        public float footUpWaitTime = .3f;
        public float logoAppearTime = 1.5f;
        public float logoWaitTime = 2f;

        [Header("Object references")]
        public ScreenFade screenFade;
        public Image character;
        public RectTransform logo;
        public AudioSource sfxScreams;
        public AudioSource sfxCarAlarm;
        public AudioSource sfxBubble;
        public AudioSource sfxStomp;

        private Rewired.Player player;

        private void Awake()
        {
            player = ReInput.players.GetPlayer(Consts.REWIRED_PLAYER_ID);
        }

        private void Start()
        {
            StartCoroutine(DoLogoScreen());
        }

        private IEnumerator DoLogoScreen()
        {
            character.sprite = characterSprites[0];
            logo.localScale = Vector3.zero;
            logo.localRotation = Quaternion.Euler(0f, 0f, logoStartRotation);
            yield return screenFade.FadeIn(fadeInTime, Color.black);
            var anim = DoLogoAnimation();
            while(anim.MoveNext())
            {
                if(player.GetButton("Cancel") || player.GetButton("Confirm") || player.GetButton("Open Menu"))
                    break;
                yield return anim.Current;
            }

            sfxScreams.FadeOut(.5f);
            sfxCarAlarm.FadeOut(.5f);
            sfxBubble.FadeOut(.5f);
            sfxStomp.FadeOut(.5f);

            yield return screenFade.FadeOut(fadeOutTime, Color.black);
            FindObjectOfType<App>().StartNewState(AppState.Title);
        }

        private IEnumerator DoLogoAnimation()
        {
            sfxScreams.Play();
            sfxCarAlarm.Play();
            yield return new WaitForSeconds(beforeAnimWaitTime);
            foreach(var i in Enumerable.Range(0, numberOfStomps))
            {
                sfxStomp.Play();
                character.sprite = characterSprites[1];
                yield return new WaitForSeconds(footDownWaitTime);
                character.sprite = characterSprites[2];
                yield return new WaitForSeconds(footLiftBetweenFramesTime);
                character.sprite = characterSprites[0];
                yield return new WaitForSeconds(footUpWaitTime);
            }
            sfxBubble.Play();
            StartCoroutine(LerpHelper.QuickTween(
                (v) => logo.localRotation = Quaternion.Euler(0f, 0f, v), logoStartRotation, 0f, logoAppearTime,
                lerpType: LerpHelper.Type.BounceOut
            ));
            yield return LerpHelper.QuickTween(
                (v) => logo.localScale = v, Vector3.zero, Vector3.one, logoAppearTime,
                lerpType: LerpHelper.Type.BounceOut
            );
            yield return new WaitForSeconds(logoWaitTime);
        }

    }
}