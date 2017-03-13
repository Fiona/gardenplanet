using System;
using System.Collections;
using UnityEngine;

namespace StrawberryNova
{
	public class Bed: MonoBehaviour, IWorldObjectScript
	{
		public bool ShouldTick()
		{
			return false;
		}	

		public IEnumerator Create()
		{
			yield return null;
		}

		public IEnumerator Spawn()
		{
			yield return null;
		}

		public IEnumerator Tick()
		{
			yield return null;
		}

		public IEnumerator PlayerInteract()
		{
			yield return null;
		}
	}
}

