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

    }
}