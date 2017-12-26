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
		public WorldObject worldObject;

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

		/*
		 * Used to get a gameobject that's to be used for the in-game appearence of this object
		 */
		public virtual GameObject GetAppearencePrefab()
		{
			return Instantiate(worldObject.objectType.prefab);
		}

		/*
		 * Gets the name that should be shown to the player
		 */
		public virtual string GetDisplayName()
		{
			return worldObject.objectType.displayName;
		}
	}
}

