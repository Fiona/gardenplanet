using System;
using System.Collections;
using LitJson;
using UnityEngine;

namespace StrawberryNova
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
        public Hashtable attributes;

        public string GetDisplayName()
        {
            return script != null ? script.GetDisplayName() : objectType.displayName;
        }

        public void SetAppearence()
        {
            if(gameObject.transform.childCount > 0)
                UnityEngine.Object.DestroyImmediate(gameObject.transform.GetChild(0).gameObject);

            var appearence = new GameObject("Appearence");
            appearence.transform.SetParent(gameObject.transform, false);

            GameObject prefab = null;
            if(script != null)
                prefab = script.GetAppearencePrefab();
            else if(objectType.prefab != null)
                prefab = UnityEngine.Object.Instantiate(objectType.prefab);

            if(prefab != null)
            {
                prefab.transform.SetParent(appearence.transform, false);
                prefab.layer = Consts.COLLISION_LAYER_WORLD_OBJECTS;
            }
        }

        public bool GetAttrBool(string key)
        {
            if(attributes[key] is bool)
                return (bool)attributes[key];
            return (bool)((JsonData)attributes[key]);
        }

        public string GetAttrString(string key)
        {
            if(attributes[key] is string)
                return (string)attributes[key];
            return (string)((JsonData)attributes[key]);
        }

        public float GetAttrFloat(string key)
        {
            if(attributes[key] is float)
                return (float)attributes[key];
            return (float)(double)((JsonData)attributes[key]);
        }

        public int GetAttrInt(string key)
        {
            if(attributes[key] is int)
                return (int)attributes[key];
            return (int)((JsonData)attributes[key]);
        }

    }
}