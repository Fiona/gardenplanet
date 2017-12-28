using System;
using System.Collections;
using System.IO;
using LitJson;
using UnityEngine;

namespace StrawberryNova
{
    public class Crop: WorldObjectScript
    {
        GameController controller;
        public void Start()
        {
            controller = FindObjectOfType<GameController>();
            SetDailyReminder();
        }

        // Harvestable when fully grown
        public override IEnumerator PlayerInteract()
        {
            if(worldObject.GetAttrFloat("growth") < 100f)
                yield break;
            controller.GivePlayerItem(worldObject.GetAttrString("type"), new Hashtable(), 1);
            controller.worldTimer.DontRemindMe(DailyGrowth);
            //controller.worldObjectManager.LateDeleteWorldObject(worldObject);
        }

        public override GameObject GetAppearencePrefab()
        {
            var cropType = worldObject.GetAttrString("type");
            var appearenceHolder = new GameObject("CropAppearence");

            // Add soil
            string soilPrefabName = "hoedsoil";
            if(worldObject.GetAttrBool("watered"))
                soilPrefabName += "_watered";
            Instantiate(
                Resources.Load<GameObject>(Path.Combine(Consts.WORLD_OBJECTS_PREFABS_PATH, soilPrefabName))
            ).transform.SetParent(appearenceHolder.transform, false);

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

            if(fullyGrown)
            {
                var interactable = additionalPrefab.AddComponent<WorldObjectInteractable>();
                interactable.SetAppearenceObject(additionalPrefab);
                interactable.worldObject = worldObject;
                additionalPrefab.layer = Consts.COLLISION_LAYER_WORLD_OBJECTS;
            }

            return appearenceHolder;
        }

        public override string GetDisplayName()
        {
            return controller.itemManager.GetItemTypeByID(worldObject.GetAttrString("type")).DisplayName;
        }

        public void DailyGrowth(GameTime gameTime)
        {
            var cropType = worldObject.GetAttrString("type");

            // If not seeded yet we go away
            if(cropType == "")
            {
                controller.worldObjectManager.DeleteWorldObject(worldObject);
                return;
            }

            // Watered plants grow
            if(worldObject.GetAttrBool("watered"))
            {
                var growthMin = (float)(double)controller.globalConfig["crops"][cropType]["growth_per_day"][0];
                var growthMax = (float)(double)controller.globalConfig["crops"][cropType]["growth_per_day"][1];
                var growthAmount = UnityEngine.Random.Range(growthMin, growthMax);
                var currentGrowth = worldObject.GetAttrFloat("growth");
                worldObject.SetAttrFloat("growth", currentGrowth + growthAmount);
                if(worldObject.GetAttrFloat("growth") > 100.0f)
                    worldObject.SetAttrFloat("growth", 100f);
                // Dry up and un wilt
                worldObject.SetAttrBool("wilting", false);
                worldObject.SetAttrBool("watered", false);
            }
            else
            {
                // Unwatered plants wilt
                worldObject.SetAttrBool("wilting", true);
            }

            // Reset appearence etc
            worldObject.SetAppearence();
            SetDailyReminder();
        }

        private void SetDailyReminder()
        {
            var dailyTime = new GameTime(days: controller.worldTimer.gameTime.Days + 1, hours: Consts.CROP_GROWTH_HOUR);
            controller.worldTimer.RemindMe(dailyTime, DailyGrowth);
        }

    }
}