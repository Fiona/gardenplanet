using System;
using System.Collections;
using UnityEngine;

namespace StrawberryNova
{
	/*
	 * All scripts extend from this
	 */
	public abstract class WorldObjectScript : MonoBehaviour
	{
		/*
		 * Method that returns a boolean denoting if the Tick coroutine should
		 * be used on this world object
		 */
		public virtual bool ShouldTick()
		{
			return false;
		}

		/*
		 * Ran once when the World Object is initially created
		 */
		public virtual IEnumerator Create()
		{
			yield return null;
		}

		/*
		 * Ran everytime the object is spawned (when the player enters the map)
		 */
		public virtual IEnumerator Spawn()
		{
			yield return null;
		}

		/*
		 * Coroutine ran every frame
		 */
		public virtual IEnumerator Tick()
		{
			yield return null;
		}

		/*
		 * Ran every time the player interacts with it
		 */
		public virtual IEnumerator PlayerInteract()
		{
			yield return null;
		}
	}
}

