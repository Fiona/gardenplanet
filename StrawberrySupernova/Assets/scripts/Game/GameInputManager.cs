using System;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Rewired;
using UnityEngine.PostProcessing;

namespace StrawberryNova
{

    public class GameInputManager : MonoBehaviour
    {

        [HideInInspector]
        public bool directInputEnabled = true;
        [HideInInspector]
        public bool doingRebind;
        [HideInInspector]
        public Vector2? mouseWorldPosition;
        [HideInInspector]
        public Rewired.Player player;
        [HideInInspector]
        public bool mouseMode;
        [HideInInspector]
        public Texture2D mouseNormal;
        [HideInInspector]
        public Texture2D mouseHover;
        [HideInInspector]
        public Texture2D mouseOkay;
        [HideInInspector]
        public Texture2D mouseError;

        private GameController controller;
        private float previousScrollWheelAxis;
        private float mouseModeTime;
        private bool mouseTextureSet;
        private float lastDropTime;

        public void Awake()
        {
            player = ReInput.players.GetPlayer(Consts.REWIRED_PLAYER_ID);
            controller = FindObjectOfType<GameController>();
            SetUpMouse();
        }

        public void SetUpMouse()
        {
            mouseNormal = Resources.Load(Consts.TEXTURE_PATH_GUI_MOUSE) as Texture2D;
            mouseHover = Resources.Load(Consts.TEXTURE_PATH_GUI_MOUSE_HOVER) as Texture2D;
            mouseOkay = Resources.Load(Consts.TEXTURE_PATH_GUI_MOUSE_OKAY) as Texture2D;
            mouseError = Resources.Load(Consts.TEXTURE_PATH_GUI_MOUSE_ERROR) as Texture2D;
            SetMouseTexture(mouseNormal);
            mouseMode = false;
            mouseModeTime = Time.time - mouseModeTime;
        }

        private void LateUpdate()
        {
            if(!mouseTextureSet)
                SetMouseTexture(mouseNormal, true);
            mouseTextureSet = false;
        }

        public void Update()
        {
            if(doingRebind)
                return;

            // Checking for switching mouse mode
            Mouse mouse = ReInput.controllers.Mouse;
            if(mouseMode)
            {
                if(Mathf.Abs(mouse.screenPositionDelta.magnitude) > 1f || mouse.GetAnyButton())
                    mouseModeTime = Time.time;
                if(Time.time - mouseModeTime > 2f)
                    mouseMode = false;
                Cursor.visible = true;
            }
            else
            {
                if(Mathf.Abs(mouse.screenPositionDelta.magnitude) > 1f || mouse.GetAnyButton())
                {
                    mouseModeTime = Time.time;
                    mouseMode = true;
                }
                Cursor.visible = false;
            }

            if(!directInputEnabled)
                return;

            // Walking
            var walkHor = player.GetAxis("Move Horizontal");
            var walkVer = player.GetAxis("Move Vertical");
            var lookHor = player.GetAxis("Look Horizontal");
            var lookVer = player.GetAxis("Look Vertical");

            var directionLock = player.GetButton("Direction Lock");
            controller.player.SetDoWalk(player.GetButton("Walk"));
            if(walkHor < -0.5f)
                controller.player.WalkInDirection(Direction.Left, directionLock);
            if(walkHor > 0.5f)
                controller.player.WalkInDirection(Direction.Right, directionLock);

            if(walkVer < -0.5f)
                controller.player.WalkInDirection(Direction.Down, directionLock);
            if(walkVer > 0.5f)
                controller.player.WalkInDirection(Direction.Up, directionLock);

            // looking in direction
            if(Mathf.Abs(walkHor) < .5f && Mathf.Abs(walkVer) < .5f && (Mathf.Abs(lookHor) > .5f || Mathf.Abs(lookVer) > .5f))
            {
                controller.player.LookInDirection(new Vector3(lookHor, 0f, lookVer));
            }

            // Interacting with objects in the world
            bool collisionTest = false;
            RaycastHit hit;
            var colLayers = (1 << Consts.COLLISION_LAYER_WORLD_OBJECTS) | (1 << Consts.COLLISION_LAYER_ITEMS);

            // Hovering mouse over objects or in-world items
            if(mouseMode)
            {
                Ray ray = Camera.main.ScreenPointToRay(mouse.screenPosition);
                collisionTest = Physics.Raycast(ray, out hit, Mathf.Infinity, colLayers);

                // Get tile that the mouse is over
                controller.UpdateMouseOverTile(null);
                RaycastHit tileHit;
                if(Physics.Raycast(ray, out tileHit, Mathf.Infinity, 1 << Consts.COLLISION_LAYER_MOUSE_HOVER_PLANE))
                {
                    var rayNormal = tileHit.transform.TransformDirection(tileHit.normal);
                    if(rayNormal == tileHit.transform.up)
                    {
                        mouseWorldPosition = new Vector2(tileHit.point.x, tileHit.point.z);
                        int tileOverX = (int) ((tileHit.point.x + (Consts.TILE_SIZE / 2)) / Consts.TILE_SIZE);
                        int tileOverY = (int) ((tileHit.point.z + (Consts.TILE_SIZE / 2)) / Consts.TILE_SIZE);
                        TilePosition tilePosition = new TilePosition()
                        {
                            x = tileOverX,
                            y = tileOverY,
                            layer = controller.player.layer
                        };
                        controller.UpdateMouseOverTile(tilePosition);
                    }
                }
                else
                    mouseWorldPosition = null;
            }
            else
                // Joystick or keyboard only mode
            {
                // Pointing at nearest object
                collisionTest = Physics.SphereCast(controller.player.transform.position, .1f,
                    controller.player.transform.forward, out hit, Mathf.Infinity, colLayers);

                // Get tile in front of player
                controller.UpdateMouseOverTile(controller.player.GetTileInFrontOf());
            }

            // If we're pointing at an object with mouse or joystick/keyboard-only
            controller.noTileSelection = false;
            if(collisionTest)
            {
                // Try items first
                var itemComponent = hit.transform.gameObject.GetComponent<InWorldItem>();
                if(itemComponent != null && itemComponent.itemType.CanPickup)
                {
                    if(Vector3.Distance(controller.player.transform.position, itemComponent.transform.position) <
                       Consts.PLAYER_PICKUP_DISTANCE)
                    {
                        itemComponent.FullHighlight();
                        controller.noTileSelection = true;
                        if(player.GetButtonUp("Use Object"))
                        {
                            itemComponent.Pickup();
                            return;
                        }
                    }
                    else
                        itemComponent.Highlight();
                }
                else
                {
                    // Fallback to trying to interact with world objects
                    var worldObjectComponent =
                        hit.transform.gameObject.GetComponentInParent<WorldObjectInteractable>();
                    if(worldObjectComponent != null)
                    {
                        if(Vector3.Distance(controller.player.transform.position,
                               worldObjectComponent.gameObject.transform.position) <
                           Consts.PLAYER_INTERACT_DISTANCE)
                        {
                            worldObjectComponent.FullHighlight();
                            controller.noTileSelection = true;
                            if(player.GetButtonUp("Use Object"))
                            {
                                worldObjectComponent.InteractWith();
                                return;
                            }
                        }
                        else
                            worldObjectComponent.Highlight();
                    }
                }
            }

            // Hotbar
            for(var i = 0; i < 10; i++)
                if(player.GetButtonUp(String.Format("Hotbar Item {0}", i+1)))
                    controller.SelectHotbarItem(i);

            if(player.GetButtonDown("Previous Hotbar Item"))
                controller.SelectPreviousHotbarItem();
            if(player.GetButtonDown("Next Hotbar Item"))
                controller.SelectNextHotbarItem();

            // Menu
            if(player.GetButtonDown("Open Menu"))
            {
                StartCoroutine(controller.OpenInGameMenu());
                return;
            }

            // Interact item in hand
            if(player.GetButtonUp("Use Item"))
            {
                // If we're using the mouse, stop the player doing anything if they're over the UI
                // this is because they were clicking items on the hotbar and immediately using it
                if(mouseMode && !controller.isMouseOverWorld.isOver)
                    return;

                // Tile item
                if(!controller.noTileSelection &&
                   controller.activeTile != null &&
                   controller.itemHotbar.activeItemIsTileItem &&
                   controller.itemHotbar.CanBeUsedOnTilePos(controller.activeTile))
                {
                    StartCoroutine(controller.PlayerUseItemInHandOnTilePos(controller.activeTile));
                    return;
                }

                // Non-tile item
                if(controller.itemHotbar.CanBeUsed())
                {
                    StartCoroutine(controller.PlayerUseItemInHand());
                    return;
                }
            }

            // Drop item in hand on ground
            if(player.GetButtonUp("Drop Item") && Time.time > lastDropTime + Consts.DROP_ITEM_COOLDOWN)
            {
                lastDropTime = Time.time;
                controller.PlayerDropItemInHand();
                return;
            }

        }

        /*
         * Stops the player doing anything directly like moving the character.
         * Still allows mouse clicks to continue.
         */
        public void LockDirectInput()
        {
            directInputEnabled = false;
        }

        /*
         * Undoes LockDirectInput
         */
        public void UnlockDirectInput()
        {
            directInputEnabled = true;
        }

        public void SetMouseTexture(Texture2D texture, bool forceChange = false)
        {
            if(!directInputEnabled && !forceChange)
                return;
            mouseTextureSet = true;
            Cursor.SetCursor(texture, new Vector2(2.0f, 2.0f), CursorMode.Auto);
        }

    }

}