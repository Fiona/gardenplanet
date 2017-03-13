using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace StrawberryNova
{
	
	public class GameController: MonoBehaviour
	{

	    [Header("Object References")]
	    public PlayerCamera mainCamera;
	    public Player player;
		public GameObject worldObjectPopup;
		public Text worldObjectPopupText;

	    [HideInInspector]
	    public Tilemap tilemap;
	    [HideInInspector]
	    public TileTypeSet tileTypeSet;
	    [HideInInspector]
	    public Map map;
		[HideInInspector]
		public MarkerManager markerManager;
		[HideInInspector]
		public WorldObjectManager worldObjectManager;

		bool showPopup;
		ObjectWorldPosition objectCurrentlyInteractingWith;

	    public void Awake()
	    {
			// Create required objects
	        tileTypeSet = new TileTypeSet("default");
	        map = new Map("main");

	        var tilemapObj = new GameObject("Tilemap");
	        tilemap = tilemapObj.AddComponent<Tilemap>();
	        tilemap.LoadFromMap(map, tileTypeSet);

			var markerManagerObj = new GameObject("MarkerManager");
			markerManager = markerManagerObj.AddComponent<MarkerManager>();
			markerManager.LoadFromMap(map);

			var worldObjectManagerObj = new GameObject("WorldObjectManager");
			worldObjectManager = worldObjectManagerObj.AddComponent<WorldObjectManager>();
			worldObjectManager.LoadFromMap(map);

			worldObjectPopup.SetActive(false);

			// Set up player and camera
			var playerStartMarker = markerManager.GetFirstTileMarkerOfType("PlayerStart");
			if(playerStartMarker != null)
				player.SetPositionToTile(playerStartMarker);
			else
				player.SetPositionToTile(new ObjectTilePosition{x=0, y=0, layer=0, dir=Direction.Down});
			mainCamera.SetTarget(player.gameObject, Consts.CAMERA_PLAYER_DISTANCE);
			mainCamera.LockTarget(player.gameObject, Consts.CAMERA_PLAYER_DISTANCE, 5.0f);
	    }
			
		public void LateUpdate()
		{
			if(!showPopup)
				worldObjectPopup.SetActive(false);
			showPopup = false;
		}

		public void ShowPopup(string textToShow)
		{
			worldObjectPopupText.text = worldObjectManager.GetWorldObjectTypeByName(textToShow).displayName;
			worldObjectPopup.SetActive(true);
			showPopup = true;
		}
			
		public IEnumerator PlayerInteractWith(ObjectWorldPosition worldObject)
		{
			if(objectCurrentlyInteractingWith != null)
				yield return null;
			objectCurrentlyInteractingWith = worldObject;
			yield return StartCoroutine(player.TurnTowardsWorldObject(worldObject));
			objectCurrentlyInteractingWith = null;			
		}
			
	}

}