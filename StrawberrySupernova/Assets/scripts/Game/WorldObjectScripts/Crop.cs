using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEngine;

namespace StrawberryNova
{
    public class Crop: WorldObjectScript
    {
        private GameController controller;
        private float cropRotation;

        public void Start()
        {
            controller = FindObjectOfType<GameController>();
            cropRotation = UnityEngine.Random.Range(0, 360);
            SetDailyReminder();
        }

        // Harvestable when fully grown
        public override IEnumerator PlayerInteract()
        {
            if(worldObject.GetAttrFloat("growth") < 100f)
                yield break;
            // Try to give the player theh produce, if it worked, reset values and appearence
            // so we go back to hoed ground
            if(controller.GivePlayerItem(worldObject.GetAttrString("type")))
                ResetToSoil();
        }

        public override void OnDestroy()
        {
            controller.worldTimer.DontRemindMe(DailyGrowth);
        }

        public override GameObject GetAppearencePrefab(bool isNew = false)
        {
            if(controller == null)
                controller = FindObjectOfType<GameController>();

            var cropType = worldObject.GetAttrString("type");
            var appearenceHolder = new GameObject("CropAppearence");

            // Add soil
            var soilObject = Instantiate(
                Resources.Load<GameObject>(Path.Combine(Consts.WORLD_OBJECTS_PREFABS_PATH, "hoedsoil"))
            );
            soilObject.transform.SetParent(appearenceHolder.transform, false);
            controller.autoTileManager.SetMaterialOfSoil(
                soilObject,
                new TilePosition(new WorldPosition(worldObject.x, worldObject.y, worldObject.height)),
                worldObject.GetAttrBool("watered"),
                isNew
            );

            // If nothing else this is all we want
            if(cropType == "")
                return appearenceHolder;

            // If it's been planted on, add additional visuals
            var fullyGrown = false;
            var additionalPrefabName = "planted_seeds";
            if(worldObject.GetAttrFloat("growth") >= 100f)
            {
                // Finished one is interactable
                additionalPrefabName = String.Format("crop_{0}_grown", cropType);
                fullyGrown = true;
            }
            else if(Math.Abs(worldObject.GetAttrFloat("growth")) > 0.05)
            {
                // Work out which stage to display
                var numStages = (int)controller.globalConfig["crops"][cropType]["num_growth_stages"];
                var stageAt = Math.Ceiling(numStages * (worldObject.GetAttrFloat("growth") / 100f));
                additionalPrefabName = String.Format("crop_{0}_{1}", cropType, stageAt);
                // Show wilted version if applicable
                if(worldObject.GetAttrBool("wilting"))
                    additionalPrefabName += "_wilting";
            }

            var additionalPrefab = Instantiate(
                Resources.Load<GameObject>(Path.Combine(Consts.WORLD_OBJECTS_PREFABS_PATH, additionalPrefabName))
            );
            additionalPrefab.transform.SetParent(appearenceHolder.transform, false);
            additionalPrefab.transform.localRotation = Quaternion.Euler(0f, cropRotation, 0f);

            if(fullyGrown)
            {
                var interactable = additionalPrefab.AddComponent<WorldObjectInteractable>();
                interactable.SetAppearenceObject(additionalPrefab);
                interactable.worldObject = worldObject;
                additionalPrefab.layer = Consts.COLLISION_LAYER_WORLD_OBJECTS;
            }

            return appearenceHolder;
        }

        public override InfoPopup.InfoPopupDisplay GetInfoPopup()
        {
            if(worldObject.GetAttrString("type") == "")
                return new InfoPopup.InfoPopupDisplay();

            var displayName = controller.itemManager.GetItemTypeByID(worldObject.GetAttrString("type")).DisplayName;

            // Finished crops inform the player
            if(worldObject.GetAttrFloat("growth") >= 100.0f)
                return new InfoPopup.InfoPopupDisplay{
                    Text = displayName,
                    ExtraText = "Ready to harvest!",
                    Icon = InfoPopup.ExtraInfoIcon.HAPPY
                };

            // Extra hints of crop behaviour
            var extraInfo = "";
            if(worldObject.GetAttrFloat("growth") < 1f && !IsCorrectSeason())
                extraInfo = "Wont sprout in this season!";
            else if(Math.Abs(worldObject.GetAttrFloat("growth")) > 0.05 && worldObject.GetAttrBool("wilting"))
                extraInfo = "Wilting - please water me!";

            return new InfoPopup.InfoPopupDisplay{
                Text = displayName,
                ExtraText = extraInfo
            };
        }

        public void DailyGrowth(GameTime gameTime)
        {
            var cropType = worldObject.GetAttrString("type");

            // If not seeded yet there's a chance that we go away
            if(cropType == "")
            {
                // 1 in 5 chance of it being removed automatically
                if(UnityEngine.Random.Range(0f, 1f) < .2f)
                {
                    controller.worldObjectManager.DeleteWorldObject(worldObject);
                    controller.autoTileManager.UpdateTilesSurrounding(new TilePosition(worldObject.GetWorldPosition()));
                    return;
                }

                // Otherwise make sure we're dried up and keep our reminder
                if(worldObject.GetAttrBool("watered"))
                {
                    worldObject.SetAttrBool("watered", false);
                    worldObject.SetAppearence();
                }
                SetDailyReminder();
                return;
            }

            // Watered plants grow
            if(worldObject.GetAttrBool("watered"))
            {
                var willGrow = true;

                // Check if the right season if we haven't started growing yet
                if(worldObject.GetAttrFloat("growth") < 1f)
                    willGrow = IsCorrectSeason();

                // Little wheat sheaf stretches up to the sky
                if(willGrow)
                {
                    var growthMin = (float) (double) controller.globalConfig["crops"][cropType]["growth_per_day"][0];
                    var growthMax = (float) (double) controller.globalConfig["crops"][cropType]["growth_per_day"][1];
                    var growthAmount = UnityEngine.Random.Range(growthMin, growthMax);
                    var currentGrowth = worldObject.GetAttrFloat("growth");
                    worldObject.SetAttrFloat("growth", currentGrowth + growthAmount);
                    if(worldObject.GetAttrFloat("growth") > 100.0f)
                        worldObject.SetAttrFloat("growth", 100f);
                }

                // Dry up and un wilt
                worldObject.SetAttrBool("wilting", false);
                worldObject.SetAttrBool("watered", false);
            }
            else
            {
                // Unwatered plants wilt
                worldObject.SetAttrBool("wilting", true);
            }

            // Reset any damage done to it
            worldObject.SetAttrFloat("damage", 0f);

            // Reset appearence etc
            worldObject.SetAppearence();
            SetDailyReminder();
        }

        private void SetDailyReminder()
        {
            var dailyTime = new GameTime(days: controller.worldTimer.gameTime.Days + 1, hours: Consts.CROP_GROWTH_HOUR);
            controller.worldTimer.RemindMe(dailyTime, DailyGrowth);
        }

        private bool IsCorrectSeason()
        {
            // Get list of acceptable season numbers
            var cropType = worldObject.GetAttrString("type");
            JsonData seasons = controller.globalConfig["crops"][cropType]["seasons"];
            var seasonNumbers = new List<int>();
            foreach(JsonData seasonName in seasons)
                seasonNumbers.Add(Season.GetSeasonByShortName((string)seasonName));

            // First check if current season is dead-on
            if(seasonNumbers.Contains(controller.worldTimer.gameTime.DateSeason))
                return true;

            // Crops can grow past any season into the next third, so check to see if the last season was valid (if
            // we're still in the first third of current one)
            var previousSeason = (controller.worldTimer.gameTime - new GameTime(0, 0, 0, 1)).DateSeason;
            return controller.worldTimer.gameTime.DateSeasonThird == 1 && seasonNumbers.Contains(previousSeason);
        }

        private void ResetToSoil()
        {
            worldObject.SetAttrString("type", "");
            worldObject.SetAttrFloat("growth", 0f);
            worldObject.SetAttrFloat("damage", 0f);
            worldObject.SetAttrBool("wilting", false);
            worldObject.SetAttrBool("watered", false);
            worldObject.SetAppearence();
        }

    }
}