using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections;

namespace StrawberryNova
{

    public class InputManager : MonoBehaviour
    {

        [HideInInspector] public bool directInputEnabled = true;
        [HideInInspector] public Vector2? mouseWorldPosition;

        private App app;
        private MonoBehaviour controller;
        private float previousScrollWheelAxis;

        public void Awake()
        {
            app = FindObjectOfType<App>();
            SetUpMouse();
            StartCoroutine(HandlePanning());
        }

        public void SetUpMouse()
        {
            Texture2D mouseTexture = Resources.Load(Consts.TEXTURE_PATH_GUI_MOUSE) as Texture2D;
            SetMouseTexture(mouseTexture);
        }

        public void Start()
        {
            if(app.state == AppState.Editor)
                StartCoroutine(ManageInputEditor());
        }

        public void Update()
        {
            if(app.state == AppState.Game)
                DoInputGame();
        }

        private void DoInputGame()
        {

            if(controller == null)
                controller = FindObjectOfType<GameController>();
            if(controller == null)
                return;            
            var gameController = (GameController)controller;

            if(!directInputEnabled)
                return;

            // Walking
            if(Input.GetKey(KeyCode.Comma))
                gameController.player.WalkInDirection(Direction.Up);
            if(Input.GetKey(KeyCode.E))
                gameController.player.WalkInDirection(Direction.Right);
            if(Input.GetKey(KeyCode.O))
                gameController.player.WalkInDirection(Direction.Down);
            if(Input.GetKey(KeyCode.A))
                gameController.player.WalkInDirection(Direction.Left);

            // Jumping
            if(Input.GetKeyDown(KeyCode.Space))
                gameController.player.Jump();
            
            // Hovering mouse over objects or in-world items
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var colLayers = (1 << Consts.COLLISION_LAYER_WORLD_OBJECTS) | (1 << Consts.COLLISION_LAYER_ITEMS);
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, colLayers))
            {
                // Try items first
                var itemComponent = hit.transform.gameObject.GetComponent<InWorldItem>();
                if(itemComponent != null && itemComponent.itemType.CanPickup)
                {
                    if(Vector3.Distance(gameController.player.transform.position, itemComponent.transform.position) <
                       Consts.PLAYER_PICKUP_DISTANCE)
                    {
                        itemComponent.FullHighlight();
                        if(Input.GetMouseButtonUp(0))
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
                    var worldObjectComponent = hit.transform.gameObject.GetComponent<WorldObjectInteractable>();
                    if(worldObjectComponent != null)
                    {
                        if(Vector3.Distance(gameController.player.transform.position, worldObjectComponent.gameObject.transform.position) < Consts.PLAYER_INTERACT_DISTANCE)
                        {
                            worldObjectComponent.Focus();
                            if(Input.GetMouseButtonUp(0))
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
            gameController.UpdateMouseOverTile(null);
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << Consts.COLLISION_LAYER_MOUSE_HOVER_PLANE))
            {
                var rayNormal = hit.transform.TransformDirection(hit.normal);
                if(rayNormal == hit.transform.up)
                {
                    mouseWorldPosition = new Vector2(hit.point.x, hit.point.z);
                    int tileOverX = (int)((hit.point.x + (Consts.TILE_SIZE / 2)) / Consts.TILE_SIZE);
                    int tileOverY = (int)((hit.point.z + (Consts.TILE_SIZE / 2)) / Consts.TILE_SIZE);
                    TilePosition tilePosition = new TilePosition()
                    {
                        x=tileOverX,
                        y=tileOverY,
                        layer=gameController.player.layer
                    };
                    gameController.UpdateMouseOverTile(tilePosition);
                }
            }
            else
                mouseWorldPosition = null;

            // Turning
            if(mouseWorldPosition != null)
            {
                var fullPoint = new Vector3(((Vector2)mouseWorldPosition).x,
                    gameController.player.transform.position.y,
                    ((Vector2)mouseWorldPosition).y);
                gameController.player.TurnToWorldPoint(fullPoint);
            }

            // Hotbar
            if(Input.GetKeyUp(KeyCode.Alpha1))
                gameController.SelectHotbarItem(0);
            else if(Input.GetKeyUp(KeyCode.Alpha2))
                gameController.SelectHotbarItem(1);
            else if(Input.GetKeyUp(KeyCode.Alpha3))
                gameController.SelectHotbarItem(2);
            else if(Input.GetKeyUp(KeyCode.Alpha4))
                gameController.SelectHotbarItem(3);
            else if(Input.GetKeyUp(KeyCode.Alpha5))
                gameController.SelectHotbarItem(4);
            else if(Input.GetKeyUp(KeyCode.Alpha6))
                gameController.SelectHotbarItem(5);
            else if(Input.GetKeyUp(KeyCode.Alpha7))
                gameController.SelectHotbarItem(6);
            else if(Input.GetKeyUp(KeyCode.Alpha8))
                gameController.SelectHotbarItem(7);
            else if(Input.GetKeyUp(KeyCode.Alpha9))
                gameController.SelectHotbarItem(8);
            else if(Input.GetKeyUp(KeyCode.Alpha0))
                gameController.SelectHotbarItem(9);

            var axis = Input.GetAxis("Mouse ScrollWheel");

            if(Math.Abs(previousScrollWheelAxis - axis) > 0.05 && Mathf.Abs(axis) >= Consts.MOUSE_WHEEL_CLICK_SNAP)
            {
                if(axis > 0f)
                    gameController.SelectPreviousHotbarItem();
                else
                    gameController.SelectNextHotbarItem();
            }

            previousScrollWheelAxis = axis;

            // Menu
            if(Input.GetKeyUp(KeyCode.Period) || Input.GetKeyUp(KeyCode.Escape))
            {
                StartCoroutine(gameController.OpenInGameMenu());
                return;
            }

            // Interact item in hand with ground
            if(gameController.mouseOverTile != null &&
               gameController.itemHotbar.CanBeUsedOnTilePos(gameController.mouseOverTile) &&
               Input.GetMouseButtonUp(0))
            {
                StartCoroutine(gameController.PlayerUseItemInHandOnTilePos(gameController.mouseOverTile));
                return;
            }

            // Drop item in hand on ground
            if(Input.GetMouseButtonUp(1))
            {
                StartCoroutine(gameController.PlayerDropItemInHand());
                return;
            }

        }

        public IEnumerator ManageInputEditor()
        {

            MapEditorController controller = FindObjectOfType<MapEditorController>();

            while(true)
            {

                mouseWorldPosition = null;

                while(!directInputEnabled)
                    yield return new WaitForFixedUpdate();

                // Get mouse over tiles
                RaycastHit hit;
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << Consts.COLLISION_LAYER_MOUSE_HOVER_PLANE))
                {
                    var rayNormal = hit.transform.TransformDirection(hit.normal);
                    if(rayNormal == hit.transform.up)
                    {
                        mouseWorldPosition = new Vector2(hit.point.x, hit.point.z);
                        int tileOverX = (int)((hit.point.x + (Consts.TILE_SIZE / 2)) / Consts.TILE_SIZE);
                        int tileOverY = (int)((hit.point.z + (Consts.TILE_SIZE / 2)) / Consts.TILE_SIZE);
                        var tileOn = controller.tilemap.GetTileAt(tileOverX, tileOverY, controller.currentLayer);
                        if(tileOn == null)
                        {
                            controller.tilemap.MouseOverTile(null);
                            yield return new WaitForFixedUpdate();
                            continue;
                        }
                        else
                            controller.tilemap.MouseOverTile(tileOn);
                    }
                }
                else
                    controller.tilemap.MouseOverTile(null);

                yield return new WaitForFixedUpdate();

            }

        }

        public IEnumerator HandlePanning()
        {
            if(app.state != AppState.Editor)
                yield break;

            MapEditorController controller = FindObjectOfType<MapEditorController>();

            while(true)
            {
                if(Input.GetMouseButtonDown(2))
                {
                    var initialMousePosition = Input.mousePosition;
                    var pannedPosition = Vector2.zero;
                    Cursor.lockState = CursorLockMode.Locked;
                    while(true)
                    {
                        controller.PanCamera(-Input.GetAxis("Mouse X") * .5f, -Input.GetAxis("Mouse Y") * .5f);
                        if (Input.GetMouseButtonUp(2))
                        {
                            Cursor.lockState = CursorLockMode.None;
                            FindObjectOfType<InputManager>().SetUpMouse();
                            break;
                        }
                        yield return new WaitForFixedUpdate();
                    }
                }
                yield return new WaitForFixedUpdate();
            }
        }

        /*
         Stops the player doing anything directly like moving the character.
         Still allows mouse clicks to continue.
         */
        public void LockDirectInput()
        {
            directInputEnabled = false;
        }

        /*
         Undoes LockDirectInput
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