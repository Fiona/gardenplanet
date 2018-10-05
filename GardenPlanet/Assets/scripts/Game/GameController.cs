using System.Collections;
using System.IO;
using StompyBlondie;
using UnityEngine;

namespace GardenPlanet
{
    /*
     * Main controller for the game itself. Always exists and holds references to a whole load of useful objects.
     */
    public class GameController: MonoBehaviour
    {

        [Header("Object References")]
        public PlayerCamera mainCamera;
        public RectTransform canvasRect;
        public ScreenFade screenFade;
        public IsMouseOver isMouseOverWorld;

        [HideInInspector]
        public Player player;
        [HideInInspector]
        public GlobalConfig globalConfig;
        [HideInInspector]
        public ItemManager itemManager;
        [HideInInspector]
        public WorldObject objectCurrentlyInteractingWith;
        [HideInInspector]
        public ItemHotbar itemHotbar;
        [HideInInspector]
        public PlayerEnergy playerEnergy;
        [HideInInspector]
        public GameInputManager GameInputManager;
        [HideInInspector]
        public GlobalButton inGameMenuButton;
        [HideInInspector]
        public TilePosition activeTile;
        [HideInInspector]
        public bool noTileSelection;
        [HideInInspector]
        public MouseHoverPlane mouseHoverPlane;
        [HideInInspector]
        public AutoTileManager autoTileManager;
        [HideInInspector]
        public World world;
        [HideInInspector]
        public EffectsManager effectsManager;

        private Debug debugMenu;
        private InfoPopup infoPopup;
        private GameState gameState;

        public void Awake()
        {
            // Load global config
            var configFilePath = Path.Combine(Consts.DATA_DIR, Consts.FILE_GLOBAL_CONFIG);
            var jsonContents = "{}";
            if(File.Exists(configFilePath))
                using(var fh = File.OpenText(configFilePath))
                    jsonContents = fh.ReadToEnd();
            globalConfig = new GlobalConfig();
            JsonHandler.PopulateObject(jsonContents, globalConfig);

            // Grab current game state
            gameState = GameState.GetInstance();

            // Managers etc
            var mouseHoverPlane = new GameObject("Mouse Hover Plane");
            mouseHoverPlane.AddComponent<MouseHoverPlane>();

            var worldObject = new GameObject("World");
            world = worldObject.AddComponent<World>();
            player = world.AddCharacter(Consts.CHAR_ID_PLAYER) as Player;

            var itemManagerObj = new GameObject("ItemManager");
            itemManager = itemManagerObj.AddComponent<ItemManager>();

            var inputManagerObj = new GameObject("GameInputManager");
            GameInputManager = inputManagerObj.AddComponent<GameInputManager>();

            var autoTileManagerObj = new GameObject("AutoTileManager");
            autoTileManager = autoTileManagerObj.AddComponent<AutoTileManager>();

            effectsManager = EffectsManager.CreateEffectsManager();

            // GUI objects
            infoPopup = (Instantiate(Resources.Load(Consts.PREFAB_PATH_INFO_POPUP)) as GameObject).GetComponent<InfoPopup>();
            infoPopup.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            infoPopup.transform.SetSiblingIndex(infoPopup.transform.GetSiblingIndex() - 1);

            var itemHotbarObject = Instantiate(Resources.Load(Consts.PREFAB_PATH_ITEM_HOTBAR)) as GameObject;
            itemHotbarObject.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            itemHotbarObject.transform.SetSiblingIndex(itemHotbarObject.transform.GetSiblingIndex() - 1);
            itemHotbar = itemHotbarObject.GetComponent<ItemHotbar>();

            var playerEnergyObject = Instantiate(Resources.Load(Consts.PREFAB_PATH_PLAYER_ENERGY)) as GameObject;
            playerEnergyObject.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            playerEnergyObject.transform.SetSiblingIndex(itemHotbarObject.transform.GetSiblingIndex() - 1);
            playerEnergy = itemHotbarObject.GetComponent<PlayerEnergy>();

            var inGameMenuButtonObject = Instantiate(Resources.Load(Consts.PREFAB_PATH_IN_GAME_MENU_BUTTON)) as GameObject;
            inGameMenuButtonObject.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            inGameMenuButtonObject.transform.SetSiblingIndex(inGameMenuButtonObject.transform.GetSiblingIndex() - 1);
            inGameMenuButton = inGameMenuButtonObject.GetComponent<GlobalButton>();
            inGameMenuButton.SetCallback(() => StartCoroutine(OpenInGameMenu()));

            // Set up player and camera
            mainCamera.SetTarget(player.gameObject, Consts.CAMERA_PLAYER_DISTANCE);
            mainCamera.LockTarget(player.gameObject, Consts.CAMERA_PLAYER_DISTANCE, Consts.CAMERA_PLAYER_LOCK_SPEED);

            // Start at day...
            world.timer.gameTime += new GameTime(hours: Consts.PLAYER_WAKE_HOUR);

            // Optional debug menu
            if(Debug.isDebugBuild || Application.platform == RuntimePlatform.LinuxEditor || Application.platform == RuntimePlatform.WindowsEditor)
            {
                var debugObj = new GameObject("DebugMenu");
                debugObj.AddComponent<DebugMenu>();
            }
        }

        public void Start()
        {
            // Set player position
            var playerStartMarker = world.markers.GetFirstTileMarkerOfType(
                world.markers.GetTileMarkerTypeByName("PlayerStart")
                );
            if(playerStartMarker != null)
                player.SetPositionToTile(playerStartMarker);
            else
                player.SetPositionToTile(new ObjectTilePosition{x=0, y=0, layer=0, dir=EightDirection.Down});

            // Objects are set up, tell the game state to set the specifics up
            gameState.InitialiseGame(player);
            StartCoroutine(screenFade.FadeIn(1f));
            StartCoroutine(ControllerCoroutine());
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
            activeTile = tilePosition;
        }

        public void ShowInfoPopup(TilePosition tilePos, InfoPopup.InfoPopupDisplay infoPopupDisplay)
        {
            infoPopup.Show(tilePos, infoPopupDisplay);
        }

        public void ShowInfoPopup(WorldPosition tilePos, InfoPopup.InfoPopupDisplay infoPopupDisplay)
        {
            infoPopup.Show(tilePos, infoPopupDisplay);
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

        /*
         * Attempts to have the player holding the item matching the passed ItemType object and attrbutes.
         * Returns true if the item was successfully placed in their hand.
         */
        public bool PlayerStartHoldingItem(ItemType itemType, Attributes attributes)
        {
            return player.StartHoldingItem(itemType, attributes);
        }

        /*
         * Tells the player to stop holding something, they will place it back in their inventory.
         * This is different to dropping which can be done by PlayerDropItemInHand
         */
        public void PlayerStopHoldingItem()
        {
            player.StopHoldingItem();
        }

        public IEnumerator PlayerUseItemInHand()
        {
            yield return StartCoroutine(itemHotbar.UseItemInHand());
        }

        public IEnumerator PlayerUseItemInHandOnTilePos(TilePosition tilePos)
        {
            yield return StartCoroutine(itemHotbar.UseItemInHandOnTilePos(tilePos));
        }

        public bool PlayerDropItemInHand()
        {
            if(!itemHotbar.RemoveItemInHand())
                return false;
            player.DropHoldingItem();
            itemHotbar.UpdateItemInHand();
            return true;
        }

        public IEnumerator PlayerDoEat()
        {
            yield return StartCoroutine(player.DoAction(CharacterAction.Eat));
        }

        /*
         * Attempts to give a player an item matching the passed ItemType object and attrbutes.
         * Returns true if the player has room in their inventory for such an item and it was added
         * successfully.
         */
        public bool GivePlayerItem(ItemType itemType, Attributes attributes = null, int quantity = 1)
        {
            return itemManager.GivePlayerItem(itemType, attributes, quantity);
        }

        /*
         * Attempts to give a player an item matching the passed ID string and attrbutes.
         * Returns true if the player has room in their inventory for such an item, the item exists and
         * that it was added successfully.
         */
        public bool GivePlayerItem(string itemTypeId, Attributes attributes = null, int quantity = 1)
        {
            return itemManager.GivePlayerItem(itemTypeId, attributes, quantity);
        }

        /*
         * Attempts to remove an item matching the passed ItemType object and attributes.
         * Returns true if the player has such an item and it was removed successfully.
         */
        public bool RemovePlayerItem(ItemType itemType, Attributes attributes = null, int quantity = 1)
        {
            return itemManager.RemovePlayerItem(itemType, attributes, quantity);
        }

        /*
         * Attempts to remove an item matching the passed ID string and attributes.
         * Returns true if the player has such an item and it was removed successfully.
         */
        public bool RemovePlayerItem(string itemTypeId, Attributes attributes = null, int quantity = 1)
        {
            return itemManager.RemovePlayerItem(itemTypeId, attributes, quantity);
        }

        /*
         * Attempts to remove an item from the inventory entry passed.
         * Returns true if the player has such an item and it was removed successfully.
         */
        public bool RemovePlayerItem(Inventory.InventoryItemEntry entry, int quantity = 1)
        {
            return itemManager.RemovePlayerItem(entry, quantity);
        }

        /*
         * Attempt to use up some energy, true if successfully reduced.
         */
        public bool ConsumePlayerEnergy(float amount)
        {
            return player.ConsumeEnergy(amount);
        }

        /*
         * Attempt to increase energy by an amount, true if successfully increased.
         */
        public bool IncreasePlayerEnergy(float amount)
        {
            return player.IncreaseEnergy(amount);
        }

        public void StartCutscene()
        {
            GameInputManager.LockDirectInput();
            world.timer.StopTimer();
        }

        public void EndCutscene()
        {
            world.timer.StartTimer();
            GameInputManager.UnlockDirectInput();
        }

        public IEnumerator PlayerSleep(GameObject bedObject)
        {
            yield return StartCoroutine(player.Sleep(bedObject));
        }

        public IEnumerator OpenInGameMenu(bool forceOpen = false)
        {
            // Cancel if input is disabled
            if(!GameInputManager.directInputEnabled && !forceOpen)
                yield break;

            // Hide and disable the in-game menu button
            var igmbCanvas = inGameMenuButton.GetComponent<CanvasGroup>();
            var igmbInputMode = inGameMenuButton.GetComponent<InputModeVisibility>();
            igmbCanvas.blocksRaycasts = false;
            var oldAlpha = igmbCanvas.alpha;
            igmbCanvas.alpha = 0f;
            igmbInputMode.enabled = false;

            // Stop direct input and pause the game
            GameInputManager.LockDirectInput();
            world.timer.StopTimer();

            // Create the in-game menu object and attach
            var inGameMenuObj = Instantiate(Resources.Load(Consts.PREFAB_PATH_IN_GAME_MENU)) as GameObject;
            inGameMenuObj.transform.SetParent(FindObjectOfType<Canvas>().transform, false);
            inGameMenuObj.transform.SetSiblingIndex(world.timer.transform.GetSiblingIndex());

            // Hand control over to the in-game menu, will return when it's closed
            yield return inGameMenuObj.GetComponent<InGameMenu>().OpenMenu();

            // Destroy the menu
            Destroy(inGameMenuObj);

            // Restart the game
            world.timer.StartTimer();
            GameInputManager.UnlockDirectInput();

            // Update the current item if the player changed what's in their hand
            itemHotbar.UpdateItemInHand();

            // Restore the in-game menu button
            igmbCanvas.blocksRaycasts = true;
            igmbCanvas.alpha = oldAlpha;
            igmbInputMode.enabled = true;
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