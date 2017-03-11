using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace StrawberryNova
{
	public class WorldObjectType
	{

		public string name;
		public GameObject prefab;

		/*
		 * Returns a List containing all available world objects.
		 */
		public static List<WorldObjectType> GetAllWorldObjectTypes()
		{
			var allObjects = new List<WorldObjectType>();
			var prefabs = Resources.LoadAll<GameObject>("worldobjects/");
			foreach(var objectType in prefabs)
			{				
				var newObjectType = new WorldObjectType
					{
						name=objectType.name,
						prefab=(GameObject)objectType
					};
				allObjects.Add(newObjectType);
			}
			// Reorder by name
			allObjects.Sort((x, y) => x.name.CompareTo(y.name));
			return allObjects;
		}

	}
}