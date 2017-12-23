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
	}
}

