using System;
using System.Collections;
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
				UnityEngine.Object.DestroyImmediate(gameObject.transform.GetChild(0));

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
	}
}

