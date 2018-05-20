using System;
using System.Collections;
using LitJson;
using UnityEngine;

namespace GardenPlanet
{
    public class WorldObject
    {
        public float x;
        public float y;
        public float height;
        public EightDirection dir;
        public string name = "";
        public GameObject gameObject;
        public WorldObjectScript script;
        public WorldObjectType objectType;
        public Attributes attributes;

        public InfoPopup.InfoPopupDisplay GetInfoPopup()
        {
            return script != null ? script.GetInfoPopup() : new InfoPopup.InfoPopupDisplay();
        }

        public void SetAppearence(bool isNew = false)
        {
            if(gameObject.transform.childCount > 0)
                UnityEngine.Object.DestroyImmediate(gameObject.transform.GetChild(0).gameObject);

            var appearence = new GameObject("Appearence");
            appearence.transform.SetParent(gameObject.transform, false);

            GameObject prefab = null;
            if(script != null)
                prefab = script.GetAppearencePrefab(isNew);
            else if(objectType.prefab != null)
                prefab = UnityEngine.Object.Instantiate(objectType.prefab);

            if(prefab != null)
            {
                prefab.transform.SetParent(appearence.transform, false);
                prefab.layer = Consts.COLLISION_LAYER_WORLD_OBJECTS;
            }
        }

        public WorldPosition GetWorldPosition()
        {
            return new WorldPosition(x, y, height);
        }

    }
}