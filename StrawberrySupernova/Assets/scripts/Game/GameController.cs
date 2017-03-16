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
		[HideInInspector]
		public WorldObject objectCurrentlyInteractingWith;
		[HideInInspector]
		public WorldTimer worldTimer;

		bool showPopup;
		Debug debugMenu;

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

			var worldTimerObject = Instantiate(Resources.Load(Consts.PREFAB_PATH_WORLD_TIMER)) as GameObject;
			worldTimerObject.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
			worldTimer = worldTimerObject.GetComponent<WorldTimer>();

			worldObjectPopup.SetActive(false);

			// Set up player and camera
			var playerStartMarker = markerManager.GetFirstTileMarkerOfType("PlayerStart");
			if(playerStartMarker != null)
				player.SetPositionToTile(playerStartMarker);
			else
				player.SetPositionToTile(new ObjectTilePosition{x=0, y=0, layer=0, dir=Direction.Down});
			mainCamera.SetTarget(player.gameObject, Consts.CAMERA_PLAYER_DISTANCE);
			mainCamera.LockTarget(player.gameObject, Consts.CAMERA_PLAYER_DISTANCE, 5.0f);

			// Optional debug menu
			if(Application.platform == RuntimePlatform.LinuxEditor)
			{
				var debugObj = new GameObject("DebugMenu");
				debugObj.AddComponent<DebugMenu>();
			}
	    }

		public void Start()
		{
			worldTimer.StartTimer();
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
			
		public IEnumerator PlayerInteractWith(WorldObject worldObject)
		{
			if(objectCurrentlyInteractingWith != null)
				yield return null;
			player.LockInput();
			worldTimer.StopTimer();
			objectCurrentlyInteractingWith = worldObject;
			yield return StartCoroutine(player.TurnTowardsWorldObject(worldObject));
			if(worldObject.script != null)
				yield return StartCoroutine(worldObject.script.PlayerInteract());
			objectCurrentlyInteractingWith = null;			
			worldTimer.StartTimer();
			player.UnlockInput();
		}

		public void PlayerDoSleep()
		{
			worldTimer.GoToNextDay();
		}
			
	}

}