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

        public override GameObject GetAppearencePrefab()
        {
            var appearenceHolder = new GameObject("CropAppearence");

            // Add soil
            string soilPrefabName = "hoedsoil";
            if(worldObject.GetAttrBool("watered"))
                soilPrefabName += "_watered";
            Instantiate(
                Resources.Load<GameObject>(Path.Combine(Consts.WORLD_OBJECTS_PREFABS_PATH, soilPrefabName))
            ).transform.SetParent(appearenceHolder.transform, false);

            // If nothing else this is all we want
            if(worldObject.GetAttrString("type") == "")
                return appearenceHolder;

            // If it's been planted on, add additional visuals
            var additionalPrefabName = "planted_seeds";
            if(Math.Abs(worldObject.GetAttrFloat("growth")) > 0.05)
            {
                additionalPrefabName = "crop_cabbage_1";
            }
            Instantiate(
                Resources.Load<GameObject>(Path.Combine(Consts.WORLD_OBJECTS_PREFABS_PATH, additionalPrefabName))
            ).transform.SetParent(appearenceHolder.transform, false);

            return appearenceHolder;
        }

        public void DailyGrowth(GameTime gameTime)
        {
            // If not seeded yet we go away
            if(worldObject.GetAttrString("type") == "")
            {
                controller.worldObjectManager.DeleteWorldObject(worldObject);
                return;
            }

            // Dry up
            worldObject.attributes["watered"] = false;

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