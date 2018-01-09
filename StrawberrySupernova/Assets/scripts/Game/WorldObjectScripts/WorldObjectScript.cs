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
		public virtual IEnumerator OnCreate()
		{
			yield return null;
		}

		/*
		 * Called the World Object is destroyed for cleanup
		 */
		public virtual void OnDestroy()
		{
			return;
		}

		/*
		 * Ran everytime the object is spawned (when the player enters the map)
		 * for setting up animations, effects, etc.
		 */
		public virtual IEnumerator OnSpawn()
		{
			yield return null;
		}

		/*
		 * Coroutine ran every frame
		 */
		public virtual IEnumerator OnTick()
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
		 * Gets info popup for name etc
		 */
		public virtual string[] GetInfoPopup()
		{
			return new[]{ worldObject.objectType.displayName, ""};
		}
	}
}

