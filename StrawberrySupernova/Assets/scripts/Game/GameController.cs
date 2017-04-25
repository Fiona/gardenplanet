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
        public ItemManager itemManager;
		[HideInInspector]
		public WorldObject objectCurrentlyInteractingWith;
		[HideInInspector]
		public WorldTimer worldTimer;
        [HideInInspector]
        public Atmosphere atmosphere;
        [HideInInspector]
        public GameObject worldObjectPopup;
        [HideInInspector]
        public InputManager inputManager;

        Text worldObjectPopupText;
		bool showPopup;
		Debug debugMenu;

	    public void Awake()
	    {
			// Init
	        tileTypeSet = new TileTypeSet("default");
	        map = new Map("main");

            // Managers
	        var tilemapObj = new GameObject("Tilemap");
	        tilemap = tilemapObj.AddComponent<Tilemap>();
	        tilemap.LoadFromMap(map, tileTypeSet);

			var markerManagerObj = new GameObject("MarkerManager");
			markerManager = markerManagerObj.AddComponent<MarkerManager>();
			markerManager.LoadFromMap(map);

			var worldObjectManagerObj = new GameObject("WorldObjectManager");
			worldObjectManager = worldObjectManagerObj.AddComponent<WorldObjectManager>();
			worldObjectManager.LoadFromMap(map);

            var itemManagerObj = new GameObject("ItemManager");
            itemManager = itemManagerObj.AddComponent<ItemManager>();

            var inputManagerObj = new GameObject("InputManager");
            inputManager = inputManagerObj.AddComponent<InputManager>();

            // GUI objects
			var worldTimerObject = Instantiate(Resources.Load(Consts.PREFAB_PATH_WORLD_TIMER)) as GameObject;
			worldTimerObject.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            worldTimerObject.transform.SetSiblingIndex(worldTimerObject.transform.GetSiblingIndex() - 1);
			worldTimer = worldTimerObject.GetComponent<WorldTimer>();

            worldObjectPopup = Instantiate(Resources.Load(Consts.PREFAB_PATH_WORLD_OBJECT_POPUP)) as GameObject;
            worldObjectPopup.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            worldTimerObject.transform.SetSiblingIndex(worldObjectPopup.transform.GetSiblingIndex() - 1);
            worldObjectPopup.SetActive(false);
            worldObjectPopupText = worldObjectPopup.GetComponentInChildren<Text>();

            // World objects
            var atmosphereObj = Instantiate(Resources.Load(Consts.PREFAB_PATH_ATMOSPHERE)) as GameObject;
            atmosphere = atmosphereObj.GetComponent<Atmosphere>();

			// Set up player and camera
			var playerStartMarker = markerManager.GetFirstTileMarkerOfType("PlayerStart");
			if(playerStartMarker != null)
				player.SetPositionToTile(playerStartMarker);
			else
				player.SetPositionToTile(new ObjectTilePosition{x=0, y=0, layer=0, dir=Direction.Down});
			mainCamera.SetTarget(player.gameObject, Consts.CAMERA_PLAYER_DISTANCE);
			mainCamera.LockTarget(player.gameObject, Consts.CAMERA_PLAYER_DISTANCE, 5.0f);

			// Optional debug menu
			//if(Application.platform == RuntimePlatform.LinuxEditor)
			//{
				var debugObj = new GameObject("DebugMenu");
				debugObj.AddComponent<DebugMenu>();
            //}
	    }

		public void Start()
		{
			worldTimer.StartTimer();
            StartCoroutine(ControllerCoroutine());
		}
			
		public void LateUpdate()
		{
			if(!showPopup)
				worldObjectPopup.SetActive(false);
			showPopup = false;
		}

        /*
         * TODO TODO
         */
        public IEnumerator ControllerCoroutine()
        {
            yield return new WaitForFixedUpdate();
        }

		public void ShowPopup(string textToShow)
		{
			worldObjectPopupText.text = worldObjectManager.GetWorldObjectTypeByName(textToShow).displayName;
			worldObjectPopup.SetActive(true);
			showPopup = true;
		}
			
		public IEnumerator PlayerInteractWith(WorldObject worldObject)
		{
			// Can't do if already messing with something
			if(objectCurrentlyInteractingWith != null)
				yield return null;
			// Set up for interaction
			StartCutscene();
			objectCurrentlyInteractingWith = worldObject;
			// Make the player look at the object
			yield return StartCoroutine(player.TurnTowardsWorldObject(worldObject));
			// Run a script that is on the object
			if(worldObject.script != null)
				yield return StartCoroutine(worldObject.script.PlayerInteract());
			// Clean up and return control to player
			objectCurrentlyInteractingWith = null;			
			EndCutscene();
		}

		public void StartCutscene()
		{
            inputManager.LockDirectInput();
			worldTimer.StopTimer();
		}

		public void EndCutscene()
		{
			worldTimer.StartTimer();
            inputManager.UnlockDirectInput();
		}

		public void PlayerDoSleep()
		{
			worldTimer.GoToNextDay(Consts.PLAYER_WAKE_HOUR);
		}

        public IEnumerator OpenInGameMenu()
        {
            inputManager.LockDirectInput();
            worldTimer.StopTimer();
            worldTimer.GetComponent<CanvasGroup>().alpha = 0;

            var inGameMenuObj = Instantiate(Resources.Load(Consts.PREFAB_PATH_IN_GAME_MENU)) as GameObject;
            inGameMenuObj.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            inGameMenuObj.transform.SetSiblingIndex(inGameMenuObj.transform.GetSiblingIndex() - 1);
            yield return inGameMenuObj.GetComponent<InGameMenu>().OpenMenu();
            Destroy(inGameMenuObj);

            worldTimer.GetComponent<CanvasGroup>().alpha = 1;
            worldTimer.StartTimer();
            inputManager.UnlockDirectInput();
        }
			
	}

}