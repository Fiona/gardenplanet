using System.Text;
using UnityEngine;
using Rewired;

namespace StrawberryNova
{

    public class GameInputManager : MonoBehaviour
    {

        [HideInInspector]
        public bool directInputEnabled = true;
        [HideInInspector]
        public Vector2? mouseWorldPosition;
        [HideInInspector]
        public Rewired.Player player;

        private GameController controller;
        private float previousScrollWheelAxis;
        private bool mouseMode;
        private float mouseModeTime;

        public void Awake()
        {
            player = ReInput.players.GetPlayer(Consts.REWIRED_PLAYER_ID);
            controller = FindObjectOfType<GameController>();
            SetUpMouse();
        }

        public void SetUpMouse()
        {
            Texture2D mouseTexture = Resources.Load(Consts.TEXTURE_PATH_GUI_MOUSE) as Texture2D;
            SetMouseTexture(mouseTexture);
            mouseMode = true;
            mouseModeTime = Time.time;
        }

        public void Update()
        {
            if(!directInputEnabled)
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

            // Walking
            var hor = player.GetAxis("Move Horizontal");
            var ver = player.GetAxis("Move Vertical");
            if(hor < -0.5f)
                controller.player.WalkInDirection(Direction.Left);
            if(hor > 0.5f)
                controller.player.WalkInDirection(Direction.Right);

            if(ver < -0.5f)
                controller.player.WalkInDirection(Direction.Down);
            if(ver > 0.5f)
                controller.player.WalkInDirection(Direction.Up);

            // Hovering mouse over objects or in-world items
            if(mouseMode)
            {

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(mouse.screenPosition);
                var colLayers = (1 << Consts.COLLISION_LAYER_WORLD_OBJECTS) | (1 << Consts.COLLISION_LAYER_ITEMS);
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, colLayers))
                {
                    // Try items first
                    var itemComponent = hit.transform.gameObject.GetComponent<InWorldItem>();
                    if(itemComponent != null && itemComponent.itemType.CanPickup)
                    {
                        if(Vector3.Distance(controller.player.transform.position, itemComponent.transform.position) <
                           Consts.PLAYER_PICKUP_DISTANCE)
                        {
                            itemComponent.FullHighlight();
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

                // Get mouse over tiles
                controller.UpdateMouseOverTile(null);
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << Consts.COLLISION_LAYER_MOUSE_HOVER_PLANE))
                {
                    var rayNormal = hit.transform.TransformDirection(hit.normal);
                    if(rayNormal == hit.transform.up)
                    {
                        mouseWorldPosition = new Vector2(hit.point.x, hit.point.z);
                        int tileOverX = (int) ((hit.point.x + (Consts.TILE_SIZE / 2)) / Consts.TILE_SIZE);
                        int tileOverY = (int) ((hit.point.z + (Consts.TILE_SIZE / 2)) / Consts.TILE_SIZE);
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

            // Turning
            // TODO: Remove
            /*
            if(mouseWorldPosition != null)
            {
                var fullPoint = new Vector3(((Vector2)mouseWorldPosition).x,
                    controller.player.transform.position.y,
                    ((Vector2)mouseWorldPosition).y);
                controller.player.TurnToWorldPoint(fullPoint);
            }*/

            // Hotbar
            /*
            if(Input.GetKeyUp(KeyCode.Alpha1))
                controller.SelectHotbarItem(0);
            else if(Input.GetKeyUp(KeyCode.Alpha3))
                controller.SelectHotbarItem(2);
            else if(Input.GetKeyUp(KeyCode.Alpha2))
                controller.SelectHotbarItem(1);
            else if(Input.GetKeyUp(KeyCode.Alpha4))
                controller.SelectHotbarItem(3);
            else if(Input.GetKeyUp(KeyCode.Alpha5))
                controller.SelectHotbarItem(4);
            else if(Input.GetKeyUp(KeyCode.Alpha6))
                controller.SelectHotbarItem(5);
            else if(Input.GetKeyUp(KeyCode.Alpha7))
                controller.SelectHotbarItem(6);
            else if(Input.GetKeyUp(KeyCode.Alpha8))
                controller.SelectHotbarItem(7);
            else if(Input.GetKeyUp(KeyCode.Alpha9))
                controller.SelectHotbarItem(8);
            else if(Input.GetKeyUp(KeyCode.Alpha0))
                controller.SelectHotbarItem(9);
*/
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
                // Tile item
                if(controller.activeTile != null &&
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
            if(player.GetButtonUp("Drop Item"))
            {
                StartCoroutine(controller.PlayerDropItemInHand());
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

        public void SetMouseTexture(Texture2D texture)
        {
            Cursor.SetCursor(texture, new Vector2(1.0f, 1.0f), CursorMode.Auto);
        }

    }

}