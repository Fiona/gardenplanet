using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.AI;
using UnityEngine.UI;

namespace StrawberryNova
{
    /*
     * GUI object that shows how much energy the player has via filled hearts
     */
    public class PlayerEnergy: MonoBehaviour
    {

        public GameObject templateHeart;
        public GameObject heartHolder;

        private GameController controller;
        private float previousPlayerEnergy;

        public void Awake()
        {
            controller = FindObjectOfType<GameController>();
            templateHeart.SetActive(false);
            previousPlayerEnergy = controller.player.currentEnergy;
            ResetHearts(controller.player.currentEnergy, controller.player.maxEnergy);
        }

        public void LateUpdate()
        {
            if(Math.Abs(controller.player.currentEnergy - previousPlayerEnergy) < 0.05f)
                return;
            ResetHearts(controller.player.currentEnergy, controller.player.maxEnergy);
            previousPlayerEnergy = controller.player.currentEnergy;
        }

        private void ResetHearts(float currentEnergy, float maxEnergy)
        {
            var numHearts = (int)Math.Ceiling(maxEnergy / Consts.GUI_ENERGY_PER_HEART);
            foreach(var i in Enumerable.Range(0, numHearts))
            {
                var thisHeartValue = currentEnergy - (Consts.GUI_ENERGY_PER_HEART * i);
                var normal = (thisHeartValue / Consts.GUI_ENERGY_PER_HEART) * 1f;
                if(normal > 1f)
                    normal = 1f;
                if(normal < 0f)
                    normal = 0f;

                // Get the heart
                GameObject heartObj;
                try
                {
                    heartObj = heartHolder.transform.GetChild(i).gameObject;
                }
                catch(UnityException)
                {
                    heartObj = Instantiate(templateHeart);
                    heartObj.SetActive(true);
                    heartObj.transform.SetParent(heartHolder.transform, false);
                }

                heartObj.GetComponent<PlayerEnergyHeart>().SetValue(normal);

                // If this heart is full and equal, we should pop it.
                if(Math.Abs(currentEnergy - Consts.GUI_ENERGY_PER_HEART*(i+1)) < 0.01f)
                    heartObj.GetComponent<PlayerEnergyHeart>().Pop(true);
            }

            // Pop the last heart if energy is full
            if(Math.Abs(currentEnergy - maxEnergy) < 0.01f)
                heartHolder.transform.GetChild(heartHolder.transform.childCount-1)
                    .GetComponent<PlayerEnergyHeart>().Pop(true);
        }

    }
}