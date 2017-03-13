using System;
using System.Collections;
using UnityEngine;

namespace StrawberryNova
{
	/*
	 * All scripts must implement this
	 */
	public interface IWorldObjectScript
	{
		/*
		 * Method that returns a boolean denoting if the Tick coroutine should
		 * be used on this world object
		 */
		bool ShouldTick();

		/*
		 * Ran once when the World Object is initially created 
		 */
		IEnumerator Create();

		/*
		 * Ran everytime the object is spawned (when the player enters the map)
		 */
		IEnumerator Spawn();

		/*
		 * Coroutine ran every frame
		 */
		IEnumerator Tick();

		/*
		 * Ran every time the player interacts with it
		 */
		IEnumerator PlayerInteract();
	}
}

