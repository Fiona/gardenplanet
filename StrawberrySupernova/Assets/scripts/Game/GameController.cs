using System;
using UnityEngine;

namespace StrawberryNova
{
	
	public class GameController: MonoBehaviour
	{

	    [Header("Object References")]
	    public PlayerCamera mainCamera;
	    public Player player;

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

			// Set up player and camera
			var playerStartMarker = markerManager.GetFirstTileMarkerOfType("PlayerStart");
			if(playerStartMarker != null)
				player.SetPositionToTile(playerStartMarker);
			else
				player.SetPositionToTile(new ObjectTilePosition{x=0, y=0, layer=0, dir=Direction.Down});
			mainCamera.SetTarget(player.gameObject, Consts.CAMERA_PLAYER_DISTANCE);
			mainCamera.LockTarget(player.gameObject, Consts.CAMERA_PLAYER_DISTANCE, 5.0f);
	    }

	}

}