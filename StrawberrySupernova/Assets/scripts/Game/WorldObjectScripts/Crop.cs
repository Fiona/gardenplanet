using System;
using System.Collections;
using System.IO;
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
            return Instantiate(
                Resources.Load<GameObject>(Path.Combine(Consts.WORLD_OBJECTS_PREFABS_PATH, "planted_seeds"))
            );
        }
        
    }
}