using System;
using StompyBlondie;
using UnityEngine;
using UnityEngine.UI;

namespace GardenPlanet
{
    /*
     * A GUI object for a single heart in the PlayerEnergy GUI object.
     */
    public class PlayerEnergyHeart: MonoBehaviour
    {
        public Image heartImage;
        private bool currentlyPopped;

        /*
         * Set the value of this single heart by passing a float between 0-1
         */
        public void SetValue(float amount)
        {
            amount = (float)Math.Floor(amount*100) / 100;
            // Animate little heart
            if(Math.Abs(heartImage.transform.localScale.x - amount) > 0.01f)
            {
                StartCoroutine(LerpHelper.QuickTween(
                    (val) => { heartImage.transform.localScale = val; },
                    heartImage.transform.localScale,
                    new Vector3(amount, amount, amount),
                    .2f
                ));
            }
            Pop(amount > 0f && amount < 1f);
        }

        /*
         * Tells the heart to appear slightly bigger to indicate it being the current heart
         */
        public void Pop(bool yes)
        {
            if(currentlyPopped == yes)
                return;

            var sizeTo = new Vector3(1.2f, 1.2f, 1.2f);

            // animate big heart
            StartCoroutine(LerpHelper.QuickTween(
                (val) => { transform.localScale = val; },
                yes ? Vector3.one : sizeTo,
                yes ? sizeTo : Vector3.one,
                .4f
            ));

            currentlyPopped = yes;
        }
    }
}