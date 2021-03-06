using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.IO;
using StompyBlondie;
using StompyBlondie.Behaviours;
using StompyBlondie.AI;
using StompyBlondie.Common.Types;

namespace GardenPlanet
{

    public class MapEditorController : Controller
    {
        private List<GameObject> barriers;
        private MouseHoverPlane mouseHoverPlane;
        private List<MapEditorMode> mapEditorModes;
        private int currentMapEditorMode = -1;
        private bool showNavigationMap;

        [HideInInspector] public int currentLayer;
        [HideInInspector] public Tilemap.Tile currentHoveredTile;
        [HideInInspector] public TileTypeSet tileTypeSet;
        [HideInInspector] public EditorInputManager editorInputManager;
        [HideInInspector] public bool showNavMap;
        [HideInInspector] public NavigationMap navigationMap;

        [Header("Editor component references")]
        public Tilemap tilemap;
        public Camera mainCamera;
        public Text layerText;
        public GameObject TilemapSelectionCube;
        public GameObject barrierTemplate;
        public Image worldPanel;
        public MainMenuBar mainMenuBar;
        public StompyBlondie.YesNoDialog yesNoDialog;
        public Map map;
        public Text currentModeText;
        public NavigationMapDebugRenderer navigationMapDebugRenderer;

        [Header("Camera")]
        public float cameraY;

        protected override void Awake()
        {
            base.Awake();
            navigationMap = new NavigationMap();
            var mouseHoverPlaneObj = new GameObject("Mouse Hover Plane");
            mouseHoverPlane = mouseHoverPlaneObj.AddComponent<MouseHoverPlane>();
            var inputManagerObj = new GameObject("Input Manager");
            editorInputManager = inputManagerObj.AddComponent<EditorInputManager>();
            barriers = new List<GameObject>();
            tileTypeSet = new TileTypeSet("default");
            cameraY = Consts.CAMERA_Y;
            navigationMapDebugRenderer.SizeOfNavPoints = 0.08f;

            try
            {
                LoadMap(null);
            }
            catch(EditorErrorException)
            {
            }

            CreateMapEditorModes();
            SelectEditorMode(0);
        }

        public void CreateMapEditorModes()
        {
            if(mapEditorModes != null)
            {
                foreach(var mode in mapEditorModes)
                {
                    mode.Destroy();
                }
            }
            mapEditorModes = new List<MapEditorMode>();
            AddMapEditorMode(new MapEditorModeTile());
            AddMapEditorMode(new MapEditorModeMarker());
            AddMapEditorMode(new MapEditorModeWorldObjects());
            AddMapEditorMode(new MapEditorModeTileTags());
        }

        private void AddMapEditorMode(MapEditorMode newMode)
        {
            newMode.Initialize();
            mapEditorModes.Add(newMode);
            newMode.Disable();
        }

        public void SelectEditorMode(int num)
        {
            if(currentMapEditorMode > -1)
                mapEditorModes[currentMapEditorMode].Disable();
            currentMapEditorMode = num;
            mapEditorModes[currentMapEditorMode].Enable();
            currentModeText.text = mapEditorModes[currentMapEditorMode].GetModeName();
        }

        public void SelectNextEditorMode()
        {
            SelectEditorMode((currentMapEditorMode + 1) % mapEditorModes.Count);
        }

        public void Update()
        {
            InputBumpScrolling();
            ClampCameraToBorders();
            mapEditorModes[currentMapEditorMode].Update();
            // Update camera position
            var y = cameraY + (Consts.TILE_SIZE * currentLayer);
            mainCamera.transform.position = new Vector3(
                mainCamera.transform.position.x,
                y,
                mainCamera.transform.position.z);
        }

        /**
         * Does mouse bumping the edge of the screen
         */
        public void InputBumpScrolling()
        {
            var body = mainCamera.GetComponent<Rigidbody>();
            Vector3? direction = null;

            if(worldPanel.GetComponent<IsMouseOver>().isOver)
            {
                if(Input.mousePosition[0] < Consts.MOUSE_BUMP_BORDER)
                    direction = Vector3.left;
                if(Input.mousePosition[0] > Screen.width - Consts.MOUSE_BUMP_BORDER)
                    direction = Vector3.right;
                if(Input.mousePosition[1] < Consts.MOUSE_BUMP_BORDER)
                    direction = Vector3.back;
                if(Input.mousePosition[1] > Screen.height - Consts.MOUSE_BUMP_BORDER - mainMenuBar.GetComponent<RectTransform>().rect.height)
                    direction = Vector3.forward;
            }

            if(direction != null)
                body.AddForce(((Vector3)direction) * Consts.MOUSE_BUMP_SPEED * Time.deltaTime);
        }


        /**
         * (Re)Initialises the map editor from a map file
         */
        public void LoadMap(string filename)
        {
            try
            {
                map = new Map(filename);
                mainMenuBar.ShowGoodMessage("Loaded map");
            }
            catch(EditorErrorException e)
            {
                mainMenuBar.ShowBadMessage(e.Message);
                return;
            }
            catch(IOException)
            {
                map = Map.NewMap(filename);
                mainMenuBar.ShowGoodMessage("Created new map");
            }
            tilemap.LoadFromMap(map, tileTypeSet);
            navigationMap = new NavigationMap(map.navigationMap);
            CreateMapEditorModes();
            SwitchToLayer(0);
            SelectEditorMode(0);
            editorInputManager.SetUpMouse();
        }

        /**
         * Saves the current Tilemap to the current Map file.
         */
        public void SaveMap()
        {
            try
            {
                map = new Map(map.filename, tilemap);
                foreach(var mode in mapEditorModes)
                    mode.SaveToMap(map);
                map.navigationMap = navigationMap;
                map.SaveMap();
            }
            catch(EditorErrorException)
            {
                return;
            }
            mainMenuBar.ShowGoodMessage("Saved map");
        }

        public void PanCamera(float vertical, float horizontal)
        {
            mainCamera.transform.position -= new Vector3(vertical, 0.0f, horizontal);
            ClampCameraToBorders();
        }

        public void ClampCameraToBorders()
        {
            mainCamera.transform.position = new Vector3(
                Mathf.Clamp(mainCamera.transform.position.x, 0, tilemap.width),
                mainCamera.transform.position.y,
                Mathf.Clamp(mainCamera.transform.position.z, -Consts.VERTICAL_EDGE_DISTANCE, tilemap.height - Consts.VERTICAL_EDGE_DISTANCE)
            );
        }

        public void ClickedOnWorld(BaseEventData data)
        {
            if(currentHoveredTile == null)
                return;

            PointerEventData pointerEventData = data as PointerEventData;

            try
            {
                var tilePos = new TilePosition{
                    x=currentHoveredTile.x,
                    y=currentHoveredTile.y,
                    layer=currentHoveredTile.layer
                };

                mapEditorModes[currentMapEditorMode].TileLocationClicked(tilePos, pointerEventData);
            }
            catch(EditorErrorException)
            {
            }
        }

        /*
          Called from Tilemap when the mouse is over a tile
         */
        public void SelectedNewTile(Tilemap.Tile selectedTile)
        {
            if(worldPanel.GetComponent<IsMouseOver>().isOver)
                currentHoveredTile = selectedTile;
            else
                currentHoveredTile = null;

            // TODO: Stop tile hover effect in world object mode
            if(currentHoveredTile == null) // || editorMode == EditorMode.WorldObject)
                TilemapSelectionCube.GetComponent<MeshRenderer>().enabled = false;
            else
            {
                TilemapSelectionCube.GetComponent<MeshRenderer>().enabled = true;
                TilemapSelectionCube.transform.localPosition = new Vector3(
                    currentHoveredTile.x * Consts.TILE_SIZE,
                    0.3f + (currentHoveredTile.layer * Consts.TILE_SIZE),
                    currentHoveredTile.y * Consts.TILE_SIZE
                );
            }
        }

        /*
          Switches to the specified map layer
         */
        private void SwitchToLayer(int layer)
        {
            currentLayer = layer;
            layerText.text = String.Format("{0}", layer);
            CreateBarrier();
            tilemap.GenerateEmptyTiles(currentLayer);
            mouseHoverPlane.RecreateCollisionPlane(tilemap);
        }

        public void ResizeTilemapTo(int width, int height)
        {
            if(width <= 0 || height <= 0)
                throw new Exception("Values must be higher than 0.");
            tilemap.SetSize(width, height);
            CreateBarrier();
            tilemap.GenerateEmptyTiles(currentLayer);
            foreach(var mode in mapEditorModes)
                mode.ResizeMap(width, height);
        }

        public void CreateBarrier()
        {

            // Delete old barriers
            foreach(var barrier in barriers)
                Destroy(barrier);
            barriers = new List<GameObject>();

            // Top of barrier
            var topBarrier = Instantiate(barrierTemplate);
            topBarrier.transform.parent = tilemap.transform;
            topBarrier.transform.localScale = new Vector3(
                Consts.TILE_SIZE * tilemap.width,
                topBarrier.transform.localScale.y,
                topBarrier.transform.localScale.z
            );
            topBarrier.transform.localPosition = new Vector3(
                (Consts.TILE_SIZE * tilemap.width / 2.0f) - (Consts.TILE_SIZE/2),
                (currentLayer * Consts.TILE_SIZE) + (Consts.TILE_SIZE/2),
                (Consts.TILE_SIZE * tilemap.height) - (Consts.TILE_SIZE/2)
            );
            barriers.Add(topBarrier);

            // Bottom of barrier
            var bottomBarrier = Instantiate(barrierTemplate);
            bottomBarrier.transform.parent = tilemap.transform;
            bottomBarrier.transform.localScale = new Vector3(
                Consts.TILE_SIZE * tilemap.width,
                bottomBarrier.transform.localScale.y,
                bottomBarrier.transform.localScale.z
            );
            bottomBarrier.transform.localPosition = new Vector3(
                (Consts.TILE_SIZE * tilemap.width / 2.0f) - (Consts.TILE_SIZE/2),
                (currentLayer * Consts.TILE_SIZE) + (Consts.TILE_SIZE/2),
                -(Consts.TILE_SIZE/2)
            );
            barriers.Add(bottomBarrier);

            // Left barrier
            var leftBarrier = Instantiate(barrierTemplate);
            leftBarrier.transform.parent = tilemap.transform;
            leftBarrier.transform.localScale = new Vector3(
                Consts.TILE_SIZE * tilemap.height,
                leftBarrier.transform.localScale.y,
                leftBarrier.transform.localScale.z
            );
            leftBarrier.transform.localPosition = new Vector3(
                -(Consts.TILE_SIZE/2),
                (currentLayer * Consts.TILE_SIZE) + (Consts.TILE_SIZE/2),
                (Consts.TILE_SIZE * tilemap.height / 2.0f) - (Consts.TILE_SIZE/2)
            );
            leftBarrier.transform.Rotate(new Vector3(0f, 0f, 90f));
            barriers.Add(leftBarrier);

            // Riiight
            var rightBarrier = Instantiate(barrierTemplate);
            rightBarrier.transform.parent = tilemap.transform;
            rightBarrier.transform.localScale = new Vector3(
                Consts.TILE_SIZE * tilemap.height,
                rightBarrier.transform.localScale.y,
                rightBarrier.transform.localScale.z
            );
            rightBarrier.transform.localPosition = new Vector3(
                (Consts.TILE_SIZE * tilemap.width) - (Consts.TILE_SIZE/2),
                (currentLayer * Consts.TILE_SIZE) + (Consts.TILE_SIZE/2),
                (Consts.TILE_SIZE * tilemap.height / 2.0f) - (Consts.TILE_SIZE/2)
            );
            rightBarrier.transform.Rotate(new Vector3(0f, 0f, -90f));
            barriers.Add(rightBarrier);

        }

        /*
          Pressed button that makes Z layer go up
         */
        public void LayerUpButtonPressed()
        {
            SwitchToLayer(currentLayer + 1);
        }

        /*
          Pressed button that makes Z layer go down
         */
        public void LayerDownButtonPressed()
        {
            SwitchToLayer(currentLayer - 1);
        }

        /*
         * Regenerating the nav map from nothing
         */
        public void RegenNavigationMap()
        {
            mainMenuBar.ShowMehMessage("Regenerating navigation map...");
            navigationMap.Reset();
            foreach(var tile in tilemap.tilemap)
            {
                if(tile.emptyTile)
                    continue;
                navigationMap.SuperimposeNavigationMap(
                    tile.tileType.navigationMap,
                    new Pos(tile.x* Consts.TILE_SIZE, tile.y* Consts.TILE_SIZE, tile.layer* Consts.TILE_SIZE),
                    tile.direction,
                    new Pos(
                        (float)tile.tileType.xCentre, (float)tile.tileType.yCentre, (float)tile.tileType.zCentre
                    )
                );
            }
            mainMenuBar.ShowMehMessage("Finished regenerating navigation map.");
        }

        /*
         * Toggles the nav map viewable off and on
         */
        public void ToggleNavmapDisplay()
        {
            showNavigationMap = !showNavigationMap;
            navigationMapDebugRenderer.navigationMap = navigationMap;
            navigationMapDebugRenderer.gameObject.SetActive(showNavigationMap);
        }

    }

}