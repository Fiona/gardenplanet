using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.AI;
using UnityEngine.UI;

namespace StrawberryNova
{
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
            foreach(Transform child in heartHolder.transform)
                if(child.gameObject.activeSelf)
                    Destroy(child.gameObject);

            var numHearts = (int)Math.Ceiling(maxEnergy / Consts.GUI_ENERGY_PER_HEART);
            foreach(var i in Enumerable.Range(0, numHearts))
            {
                var thisHeartValue = currentEnergy - (Consts.GUI_ENERGY_PER_HEART * i);
                var normal = (thisHeartValue / Consts.GUI_ENERGY_PER_HEART) * 1f;
                if(normal > 1f)
                    normal = 1f;
                if(normal < 0f)
                    normal = 0f;

                var newHeart = Instantiate(templateHeart);
                newHeart.SetActive(true);
                newHeart.transform.SetParent(heartHolder.transform, false);
                newHeart.GetComponent<PlayerEnergyHeart>().SetValue(normal);
            }

            if(Math.Abs(currentEnergy - maxEnergy) < 0.05f)
                heartHolder.transform.GetChild(heartHolder.transform.childCount-1)
                    .GetComponent<PlayerEnergyHeart>().Pop(true);
        }

    }
}