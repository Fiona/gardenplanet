using System;
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
		public IWorldObjectScript script;
		public WorldObjectType objectType;
	}
}

