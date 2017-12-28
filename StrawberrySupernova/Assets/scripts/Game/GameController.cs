using System;
using System.Collections;
using System.IO;
using System.Linq;
using LitJson;
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
        public JsonData globalConfig;
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
        public TileTagManager tileTagManager;
        [HideInInspector]
        public ItemManager itemManager;
        [HideInInspector]
        public WorldObject objectCurrentlyInteractingWith;
        [HideInInspector]
        public WorldTimer worldTimer;
        [HideInInspector]
        public ItemHotbar itemHotbar;
        [HideInInspector]
        public Atmosphere atmosphere;
        [HideInInspector]
        public GameObject worldObjectPopup;
        [HideInInspector]
        public InputManager inputManager;
        [HideInInspector]
        public TilePosition mouseOverTile;

        GameObject inWorldItems;
        Text worldObjectPopupText;
        bool showPopup;
        Debug debugMenu;

        public void Awake()
        {
            // Load global config
            var configFilePath = Path.Combine(Consts.DATA_DIR, Consts.FILE_GLOBAL_CONFIG);
            var jsonContents = "{}";
            if(File.Exists(configFilePath))
                using(var fh = File.OpenText(configFilePath))
                    jsonContents = fh.ReadToEnd();
            globalConfig = JsonMapper.ToObject(jsonContents);

            // Init
            tileTypeSet = new TileTypeSet("default");
            map = new Map("devtest");

            // Managers etc
            var mouseHoverPlane = new GameObject("Mouse Hover Plane");
            mouseHoverPlane.AddComponent<MouseHoverPlane>();

            var tilemapObj = new GameObject("Tilemap");
            tilemap = tilemapObj.AddComponent<Tilemap>();
            tilemap.LoadFromMap(map, tileTypeSet);

            var markerManagerObj = new GameObject("MarkerManager");
            markerManager = markerManagerObj.AddComponent<MarkerManager>();
            markerManager.LoadFromMap(map);

            var worldObjectManagerObj = new GameObject("WorldObjectManager");
            worldObjectManager = worldObjectManagerObj.AddComponent<WorldObjectManager>();
            worldObjectManager.LoadFromMap(map);

            var tileTagManagerObj = new GameObject("TileTagManager");
            tileTagManager = tileTagManagerObj.AddComponent<TileTagManager>();
            tileTagManager.LoadFromMap(map);

            var itemManagerObj = new GameObject("ItemManager");
            itemManager = itemManagerObj.AddComponent<ItemManager>();

            var inputManagerObj = new GameObject("InputManager");
            inputManager = inputManagerObj.AddComponent<InputManager>();

            inWorldItems = new GameObject("In World Items");

            // GUI objects
            var worldTimerObject = Instantiate(Resources.Load(Consts.PREFAB_PATH_WORLD_TIMER)) as GameObject;
            worldTimerObject.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            worldTimerObject.transform.SetSiblingIndex(worldTimerObject.transform.GetSiblingIndex() - 1);
            worldTimer = worldTimerObject.GetComponent<WorldTimer>();

            worldObjectPopup = Instantiate(Resources.Load(Consts.PREFAB_PATH_WORLD_OBJECT_POPUP)) as GameObject;
            worldObjectPopup.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            worldObjectPopup.transform.SetSiblingIndex(worldObjectPopup.transform.GetSiblingIndex() - 1);
            worldObjectPopup.SetActive(false);
            worldObjectPopupText = worldObjectPopup.GetComponentInChildren<Text>();

            var itemHotbarObject = Instantiate(Resources.Load(Consts.PREFAB_PATH_ITEM_HOTBAR)) as GameObject;
            itemHotbarObject.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            itemHotbarObject.transform.SetSiblingIndex(itemHotbarObject.transform.GetSiblingIndex() - 1);
            itemHotbar = itemHotbarObject.GetComponent<ItemHotbar>();

            // Atmosphere
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

            // Start at day...
            worldTimer.gameTime += new GameTime(hours: Consts.PLAYER_WAKE_HOUR);

            // Optional debug menu
            if(Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.WindowsEditor)
            {
                var debugObj = new GameObject("DebugMenu");
                debugObj.AddComponent<DebugMenu>();
            }
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

        public void UpdateMouseOverTile(TilePosition tilePosition)
        {
            mouseOverTile = tilePosition;
        }

        public void ShowPopup(string textToShow)
        {
            worldObjectPopupText.text = textToShow;
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
            Debug.Log("finished interaction");
            objectCurrentlyInteractingWith = null;
            EndCutscene();
        }

        public IEnumerator PlayerUseItemInHandOnTilePos(TilePosition tilePos)
        {
            yield return StartCoroutine(itemHotbar.UseItemInHandOnTilePos(tilePos));
        }

        public IEnumerator PlayerDropItemInHand()
        {
            yield return StartCoroutine(itemHotbar.DropItemInHand());
        }

        public bool GivePlayerItem(ItemType itemType, Hashtable attributes = null, int quantity = 1)
        {
            return itemManager.GivePlayerItem(itemType, attributes, quantity);
        }

        public bool GivePlayerItem(string itemTypeId, Hashtable attributes = null, int quantity = 1)
        {
            return itemManager.GivePlayerItem(itemTypeId, attributes, quantity);
        }

        public bool RemovePlayerItem(ItemType itemType, Hashtable attributes = null, int quantity = 1)
        {
            return itemManager.RemovePlayerItem(itemType, attributes, quantity);
        }

        public bool RemovePlayerItem(string itemTypeId, Hashtable attributes = null, int quantity = 1)
        {
            return itemManager.RemovePlayerItem(itemTypeId, attributes, quantity);
        }


        // Generates an item in the world at the specified position
        public void SpawnItemInWorld(ItemType itemType, System.Collections.Hashtable attributes, int amount,
            WorldPosition worldPos)
        {
            var resource = Resources.Load<GameObject>(Consts.ITEMS_PREFABS_PATH + itemType.Appearance);
            if (resource == null)
            {
                resource = Resources.Load<GameObject>(Consts.ITEMS_PREFAB_MISSING);
            }
            foreach (var i in Enumerable.Range(1, amount))
            {
                var newItem = Instantiate(resource);
                newItem.transform.parent = inWorldItems.transform;
                newItem.transform.localPosition = worldPos.TransformPosition();
                var newItemComponent = newItem.AddComponent<InWorldItem>();
                newItemComponent.attributes = attributes;
                newItemComponent.itemType = itemType;
            }
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

        public IEnumerator PlayerSleep()
        {
            yield return StartCoroutine(player.Sleep());
        }

        public IEnumerator OpenInGameMenu()
        {
            inputManager.LockDirectInput();
            worldTimer.StopTimer();
            worldTimer.GetComponent<CanvasGroup>().alpha = 0;
            itemHotbar.GetComponent<CanvasGroup>().alpha = 0;

            var inGameMenuObj = Instantiate(Resources.Load(Consts.PREFAB_PATH_IN_GAME_MENU)) as GameObject;
            inGameMenuObj.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            inGameMenuObj.transform.SetSiblingIndex(inGameMenuObj.transform.GetSiblingIndex() - 1);
            yield return inGameMenuObj.GetComponent<InGameMenu>().OpenMenu();
            Destroy(inGameMenuObj);

            itemHotbar.GetComponent<CanvasGroup>().alpha = 1;
            worldTimer.GetComponent<CanvasGroup>().alpha = 1;
            worldTimer.StartTimer();
            inputManager.UnlockDirectInput();
        }

        public void SelectHotbarItem(int hotbarItemNum)
        {
            itemHotbar.SelectItemIndex(hotbarItemNum);
        }

        public void SelectPreviousHotbarItem()
        {
            itemHotbar.SelectPreviousItem();
        }

        public void SelectNextHotbarItem()
        {
            itemHotbar.SelectNextItem();
        }

    }

}