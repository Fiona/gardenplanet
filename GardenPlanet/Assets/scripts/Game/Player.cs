using System;
using StompyBlondie;
using System.Collections;
using UnityEngine;

namespace GardenPlanet
{

    public class Player: Character
    {
        [HideInInspector]
        public Inventory inventory;
        public float maxEnergy;
        public float currentEnergy;

        private bool didYawn;
        private int wokeUpOnDay = -1;

        public static Appearence defaultAppearence = new Appearence
        {
            top = "ilovefarmingshirt",
            bottom = "",
            shoes = "",
            headAccessory = "",
            backAccessory = "",
            hair = "straight",

            eyebrows = "thin",
            eyes = "cute",
            mouth = "kind_smile",
            nose = "small",

            eyeColor = Color.HSVToRGB(115/255f, 186/255f, 158/255f),
            skinColor = Color.HSVToRGB(26/255f, 123/255f, 93/255f),
            hairColor = Color.HSVToRGB(23/255f, 173/255f, 229/255f),
        };
        public static Information defaultInformation = new Information
        {
            Name = "Tess",
            seasonBirthday = 1,
            dayBirthday = 1
        };

        public override void Awake()
        {
            base.Awake();
            inventory = new Inventory(Consts.PLAYER_INVENTORY_MAX_STACKS);
            maxEnergy = Consts.PLAYER_START_ENERGY;
            currentEnergy = maxEnergy;
        }

        public override void Start()
        {
            base.Start();
            SetPassOutEvent();

            // TODO: remove this test item adding
            controller.GivePlayerItem("wateringcan-level1");
            controller.GivePlayerItem("hoe-level1");
            controller.GivePlayerItem("cabbage_seeds", quantity:16);
        }

        public override void FixedUpdate()
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
            StopHoldingItem();
            yield return StartCoroutine(FindObjectOfType<StompyBlondie.ScreenFade>().FadeOut(2f));
            controller.worldTimer.GoToNextDay(Consts.PLAYER_WAKE_HOUR);
            SetPassOutEvent();
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(FindObjectOfType<StompyBlondie.ScreenFade>().FadeIn(3f));
            currentEnergy = maxEnergy;
            wokeUpOnDay = controller.worldTimer.gameTime.Days;
            passedOut = false;
            didYawn = false;
            controller.itemHotbar.UpdateItemInHand();
        }

        public IEnumerator PassOut()
        {
            // Do falling over animation
            yield return DoAction(CharacterAction.PassOut);
            controller.StartCutscene();

            // Tell the player what happened
            yield return MessagePopup.ShowMessagePopup(
                "You pass out from exhaustion..."
            );

            // Fade out
            yield return StartCoroutine(FindObjectOfType<ScreenFade>().FadeOut(4f, new Color(1f,1f,1f)));

            // Reset some game stuff, set the new day time
            controller.worldTimer.DontRemindMe(PassOutTimeEvent);
            // If we passed out during the morning of the next day then going to the next day will skip a whole day
            // so just adjust the hour forward instead
            if(controller.worldTimer.gameTime.Days == wokeUpOnDay || wokeUpOnDay == -1)
                controller.worldTimer.GoToNextDay(Consts.PLAYER_PASS_OUT_WAKE_HOUR);
            else
                controller.worldTimer.GoToHour(Consts.PLAYER_PASS_OUT_WAKE_HOUR);
            SetPassOutEvent();
            mainAnimator.SetBool("DoPassOut", false);
            face.SetFaceState(CharacterFace.FaceState.NORMAL);

            // Wait a bit and fade back in
            yield return new WaitForSeconds(2f);
            yield return StartCoroutine(FindObjectOfType<ScreenFade>().FadeIn(4f, new Color(1f,1f,1f)));

            // Put energy to 2/3rds maximum
            currentEnergy = maxEnergy * .75f;
            passedOut = false;
            didYawn = false;
            wokeUpOnDay = controller.worldTimer.gameTime.Days;
            controller.itemHotbar.UpdateItemInHand();
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
                return true;
            }

            // Do Yawn
            if(currentEnergy <= Consts.PLAYER_YAWN_ENERGY_THRESHOLD && !didYawn)
            {
                StartCoroutine(DoAction(CharacterAction.Yawn));
                didYawn = true;
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
            if(currentEnergy > Consts.PLAYER_YAWN_ENERGY_THRESHOLD && didYawn)
                didYawn = false;
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