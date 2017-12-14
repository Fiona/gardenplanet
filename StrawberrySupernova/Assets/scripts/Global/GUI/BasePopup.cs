using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace StompyBlondie
{
	/*
	 * Gets overidden by other Popup classes
	 */
	public class BasePopup: MonoBehaviour
	{
		[HideInInspector]
		public bool closePopup;

		public static T InitPopup<T>(string prefabName)
		{
			// Create prefab
			var resource = Resources.Load(prefabName) as GameObject;
			var popupObject = Instantiate(resource);

			// Add to canvas
			var canvas = FindObjectOfType<Canvas>();
			if(canvas == null)
				throw new Exception("Can't find a Canvas to attach to.");

			popupObject.transform.SetParent(canvas.transform, false);

			return popupObject.GetComponent<T>();
		}

		public IEnumerator DoPopup()
		{
			// Wait for input
			closePopup = false;
			while(!closePopup)
				yield return new WaitForFixedUpdate();

			// Remove from canvas
			Destroy(this.gameObject);			
		}		
		
		public void Update()
		{
			if(closePopup)
				return;
			if(Input.GetKeyUp(KeyCode.Space) || Input.GetKeyUp(KeyCode.Return))
				closePopup = true;
		}

		public void ClickedOnPopup()
		{
			closePopup = true;
		}
	}
}

