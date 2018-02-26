using System;
using StompyBlondie;
using System.Collections;
using UnityEngine;

namespace StrawberryNova
{

    public class Player: Character
    {
        [HideInInspector]
        public Inventory inventory;
        public float maxEnergy;
        public float currentEnergy;

        public void Awake()
        {
            base.Awake();
            inventory = new Inventory(Consts.PLAYER_INVENTORY_MAX_STACKS);
            maxEnergy = Consts.PLAYER_START_ENERGY;
            currentEnergy = maxEnergy;
        }

        public void Start()
        {
            base.Start();
            SetPassOutEvent();

            // TODO: remove this test item adding
            controller.GivePlayerItem("broken_sprinkleboy");
            controller.GivePlayerItem("broken_trowelie");
            controller.GivePlayerItem("cabbage_seeds", quantity:16);
        }

        public void FixedUpdate()
        {
            base.FixedUpdate();

            if(controller.GameInputManager == null || !controller.GameInputManager.directInputEnabled)
                return;

            // Do auto pick-up
            if(App.gameSettings.autoPickupItems)
            {
                var hits = Physics.OverlapSphere(transform.position, Consts.PLAYER_AUTO_PICKUP_RADIUS,
                    1<<Consts.COLLISION_LAYER_ITEMS);
                if(hits.Length > 0)
                {
                    foreach(var item in hits)
                    {
                        var comp = item.GetComponent<InWorldItem>();
                        if(comp == null)
                            continue;
                        comp.Pickup(autoPickup:true);
                    }
                }
            }

        }

        public IEnumerator Sleep()
        {
            controller.worldTimer.DontRemindMe(PassOutTimeEvent);
            yield return StartCoroutine(FindObjectOfType<StompyBlondie.ScreenFade>().FadeOut(2f));
            controller.worldTimer.GoToNextDay(Consts.PLAYER_WAKE_HOUR);
            SetPassOutEvent();
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(FindObjectOfType<StompyBlondie.ScreenFade>().FadeIn(3f));
            currentEnergy = maxEnergy;
        }

        public IEnumerator PassOut()
        {
            controller.StartCutscene();
            yield return MessagePopup.ShowMessagePopup(
                "You pass out from exhaustion..."
            );
            yield return StartCoroutine(FindObjectOfType<ScreenFade>().FadeOut(4f, new Color(1f,1f,1f)));
            controller.worldTimer.DontRemindMe(PassOutTimeEvent);
            controller.worldTimer.gameTime = new GameTime(
                days: controller.worldTimer.gameTime.Days + 1,
                hours: Consts.PLAYER_PASS_OUT_WAKE_HOUR
            );
            SetPassOutEvent();
            controller.worldTimer.DoTimerEvents();
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(FindObjectOfType<ScreenFade>().FadeIn(4f, new Color(1f,1f,1f)));
            currentEnergy = maxEnergy * .75f;
            controller.EndCutscene();
        }

        /*
         * Attempt to use up some energy, true if successfully reduced.
         */
        public bool ConsumeEnergy(float amount)
        {
            if(Math.Abs(currentEnergy) < 0.01f)
                return false;
            currentEnergy -= amount;
            if(currentEnergy < 0.01f)
            {
                currentEnergy = 0f;
                StartCoroutine(PassOut());
            }
            return true;
        }

        /*
         * Attempt to increase energy by an amount, true if successfully increased.
         */
        public bool IncreaseEnergy(float amount)
        {
            if(currentEnergy >= maxEnergy)
                return false;
            currentEnergy += amount;
            if(currentEnergy > maxEnergy)
                currentEnergy = maxEnergy;
            return true;
        }

        private void PassOutTimeEvent(GameTime gameTime)
        {
            StartCoroutine(PassOut());
        }

        private void SetPassOutEvent()
        {
            var passOutOn = new GameTime(days: controller.worldTimer.gameTime.Days + 1, hours: Consts.PLAYER_PASS_OUT_HOUR);
            controller.worldTimer.RemindMe(passOutOn, PassOutTimeEvent);
        }

    }

}