using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

namespace StrawberryNova
{
	
	public class MapEditorController : MonoBehaviour
	{

	    public int currentLayer;

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

		[Header("Tile mode")]
		public GameObject modeTile;
		public GameObject currentTilePanel;
	    public Text currentTileText;
	    public GameObject currentTileTemplate;
		public EditTileDialog editTileDialog;

		[Header("Marking mode")]
		public GameObject modeMarking;
		public Image currentMarkerPreview;
		public Text currentMarkerText;

		[Header("World object mode")]
		public GameObject modeWorldObject;
		public GameObject currentWorldObjectPanel;
		public Text currentWorldObjectText;
		public GameObject currentWorldObjectTemplate;
		public Button worldObjectMinorModeButton;

	    [HideInInspector]
	    public TileTypeSet tileTypeSet;
		[HideInInspector]
		public EditorMode editorMode;

		List<GameObject> barriers;
        MouseHoverPlane mouseHoverPlane;

		// Tile mode
	    string currentTileName;
	    GameObject currentTileTypeSelectedObj;
	    string previousTileType;
	    Tilemap.Tile currentHoveredTile;
	    Direction newTileDirection;

		// Marker mode
		MarkerManager markerManager;
		TileMarkerType selectedMarker;
		TileMarkerType previousMarker;

		// World object mode
		WorldObjectManager worldObjectManager;
		WorldObjectType selectedWorldObject;
		GameObject currentWorldObjectSelectedObj;
		[HideInInspector]
		public WorldObject worldObjectMoving;
		[HideInInspector]
		public int worldObjectMinorMode;

	    public void Awake()
	    {
			var markerManagerObj = new GameObject("MarkerManager");
			markerManager = markerManagerObj.AddComponent<MarkerManager>();

			var worldObjectManagerObj = new GameObject("WorldObjectManager");
			worldObjectManager = worldObjectManagerObj.AddComponent<WorldObjectManager>();

            var mouseHoverPlaneObj = new GameObject("Mouse Hover Plane");
            mouseHoverPlane = mouseHoverPlaneObj.AddComponent<MouseHoverPlane>();

	        barriers = new List<GameObject>();
	        try
	        {
	            tileTypeSet = new TileTypeSet("default");
	            LoadMap(null);
	        }
	        catch(EditorErrorException){}

	        SwitchEditorMode(EditorMode.Tile);
			SwitchWorldObjectMinorMode(0);
	    }
			
	    public void Update()
	    {
	        // Do bump scrolling
	        var body = mainCamera.GetComponent<Rigidbody>();
	        Vector3? direction = null;

			if(worldPanel.GetComponent<StompyBlondie.IsMouseOver>().isOver)
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

	        ClampCameraToBorders();

			// If let go of mouse then we stop moving any world objects we've moving
			if(Input.GetMouseButtonUp(0) && movingWorldObject)
				movingWorldObject = false;			

			// If we've hit delete and we're moving theeen... delete it
			if(worldObjectMoving != null && Input.GetKey(KeyCode.Delete))
			{
				DeleteWorldObject(worldObjectMoving);
				return;
			}

			// If we're moving an object
			if(movingWorldObject)
			{
				if(worldObjectMoving != null)
				{
					// Set the position to the tile point that the mouse is pointing at
					RaycastHit secondaryHit;
					Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
					if(Physics.Raycast(ray, out secondaryHit, Mathf.Infinity, 1 << Consts.COLLISION_LAYER_TILES))
					{						
						var objTransform = worldObjectMoving.gameObject.transform;
						// Snap to grid if holding shift
						if(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
							objTransform.position = new Vector3(
								secondaryHit.transform.position.x, objTransform.position.y, secondaryHit.transform.position.z
							);
						else
							objTransform.position = new Vector3(
								secondaryHit.point.x, objTransform.position.y, secondaryHit.point.z
							);
                        worldObjectMoving.x = objTransform.position.x;
                        worldObjectMoving.y = objTransform.position.z;
                        worldObjectMoving.height = objTransform.position.y;
					}
				}
			}

		}
			
	    public void SwitchEditorMode(EditorMode editorMode)
	    {
	        this.editorMode = editorMode;
			modeWorldObject.SetActive(false);
			modeTile.SetActive(false);
			modeMarking.SetActive(false);
			StopMovingWorldObject();
			switch(this.editorMode)
			{
				case EditorMode.WorldObject:
					modeWorldObject.SetActive(true);
					this.currentModeText.text = "World Object Mode";
					break;
				case EditorMode.Marker:
					modeMarking.SetActive(true);
					this.currentModeText.text = "Tile Marking";
					break;
				case EditorMode.Tile:
					modeTile.SetActive(true);
					this.currentModeText.text = "Tile Drawing";
					break;
			    default:
			        break;
			}
	    }

	    public void LoadMap(string filename)
	    {
	        try
	        {
	            map = new Map(filename);
	        }
	        catch(EditorErrorException)
	        {
	            return;
	        }
			tilemap.LoadFromMap(map, tileTypeSet);
			markerManager.LoadFromMap(map);
			worldObjectManager.LoadFromMap(map);
	        SetNewTileDirection(Direction.Down);
	        currentTileTemplate.SetActive(false);
			currentWorldObjectTemplate.SetActive(false);
	        SwitchToLayer(0);
	        SelectTileType(tileTypeSet.types[0].name);
			SelectMarkerType(markerManager.tileMarkerTypes[0].name);
			SelectWorldObjectType(worldObjectManager.worldObjectTypes[0].name);
			SwitchEditorMode(EditorMode.Tile);
	        mainMenuBar.ShowGoodMessage("Loaded map");
            FindObjectOfType<InputManager>().SetUpMouse();
	    }

	    // Saves the current Tilemap to the current Map file.
	    public void SaveMap()
	    {
	        try
	        {
				map = new Map(map.filename, tilemap, markerManager, worldObjectManager);
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

		private bool movingWorldObject; 

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

				// Drawing tiles
				if(editorMode == EditorMode.Tile)
				{					
					if(pointerEventData.button == PointerEventData.InputButton.Left)
					{
						tilemap.RemoveTile(tilePos.x, tilePos.y, tilePos.layer);
						tilemap.AddTile(currentTileName, tilePos.x, tilePos.y, tilePos.layer, newTileDirection);
					}
					else if(pointerEventData.button == PointerEventData.InputButton.Right)
						SelectTileType(currentHoveredTile.tileTypeName);
				}
				// Placing tile markers
				else if(editorMode == EditorMode.Marker)
				{
					if(pointerEventData.button == PointerEventData.InputButton.Left)
					{
						markerManager.RemoveMarkerAt(tilePos.x, tilePos.y, tilePos.layer);
						markerManager.AddMarkerAt(selectedMarker, tilePos.x, tilePos.y, tilePos.layer, newTileDirection);
					}
					else if(pointerEventData.button == PointerEventData.InputButton.Right)
					{
						var marker = markerManager.GetMarkerAt(
							currentHoveredTile.x, currentHoveredTile.y, currentHoveredTile.layer
						);
						SelectMarkerType(marker == null ? null : marker.name);
					}
				}
				// Wanting to add world objects
				else if(editorMode == EditorMode.WorldObject && worldObjectMinorMode == 0)
				{
					if(Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.RightShift))
						worldObjectManager.AddWorldObject(selectedWorldObject, tilePos);
					else
					{
						// Without shift being held we don't snap to grid
						RaycastHit hit;
						Ray ray = Camera.main.ScreenPointToRay(pointerEventData.position);
						if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << Consts.COLLISION_LAYER_TILES))
						{						
							var worldPos = new WorldPosition{
								x=hit.point.x,
								y=hit.point.z,
								height=(tilePos.layer * Consts.TILE_SIZE)
							};
							worldObjectManager.AddWorldObject(selectedWorldObject, worldPos);
						}
					}
				}
				// If moving world objects
				else if(editorMode == EditorMode.WorldObject && worldObjectMinorMode == 1)
				{
					// First check to see if we've clicked on a world object, this switches the selection
					RaycastHit hit;
					Ray ray = Camera.main.ScreenPointToRay(pointerEventData.position);
					if(!movingWorldObject && Physics.Raycast(ray, out hit, Mathf.Infinity, 1 << Consts.COLLISION_LAYER_WORLD_OBJECTS))
					{
						var worldObject = worldObjectManager.GetWorldObjectByGameObject(hit.transform.gameObject);
						if(worldObject != null && worldObject != worldObjectMoving)
						{
							WorldObjectMinorModeStartMovingWorldObject(worldObject);
							return;
						}
					}
					// If we're not switching selection and we already have one then holding
					// down the mouse will move the object
					if(!movingWorldObject)
						movingWorldObject = true;
				}
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
			if(worldPanel.GetComponent<StompyBlondie.IsMouseOver>().isOver)
	            currentHoveredTile = selectedTile;
	        else
	            currentHoveredTile = null;

			if(currentHoveredTile == null || editorMode == EditorMode.WorldObject)
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
	        float y = Consts.CAMERA_Y + (Consts.TILE_SIZE * layer);
	        mainCamera.transform.position = new Vector3(
	            mainCamera.transform.position.x,
	            y,
	            mainCamera.transform.position.z);
	        CreateBarrier();
	        tilemap.GenerateEmptyTiles(currentLayer);
            mouseHoverPlane.RecreateCollisionPlane();
	    }

	    /*
	      Switches selected tile type for drawing
	     */
	    private void SelectTileType(string tileTypeName)
	    {

	        if(currentTileTypeSelectedObj != null)
	        {
	            Destroy(currentTileTypeSelectedObj);
	            currentTileTypeSelectedObj = null;
	        }

	        if(tileTypeName == null)
	        {
	            currentTileName = null;
	            currentTileText.text = "Empty";
	            return;
	        }

	        currentTileTypeSelectedObj = tileTypeSet.InstantiateTile(tileTypeName);

	        currentTileTypeSelectedObj.transform.parent = currentTilePanel.transform;
	        currentTileTypeSelectedObj.AddComponent<RectTransform>();
	        var rect = currentTileTypeSelectedObj.GetComponent<RectTransform>();

	        // Base values on template
	        currentTileTypeSelectedObj.layer = currentTileTemplate.layer;
	        rect.localPosition = currentTileTemplate.GetComponent<RectTransform>().localPosition;
	        rect.localScale = currentTileTemplate.GetComponent<RectTransform>().localScale;
	        rect.localRotation = currentTileTemplate.GetComponent<RectTransform>().localRotation;

	        currentTileName = tileTypeName;
	        currentTileText.text = currentTileName;

	    }

		/*
	      Switches selected tile marker type
	     */
		private void SelectMarkerType(string markerTypeName)
		{
			if(markerTypeName == null)
			{
				currentMarkerText.text = "-";
				selectedMarker = null;
				currentMarkerPreview.gameObject.SetActive(false);
				return;
			}
			try
			{
				currentMarkerPreview.gameObject.SetActive(true);
				selectedMarker = markerManager.GetTileMarkerTypeByName(markerTypeName);
				currentMarkerPreview.sprite = selectedMarker.sprite;
			}
			catch(EditorErrorException){ }
			currentMarkerText.text = markerTypeName;
		}

	    public void SetNewTileDirection(Direction direction)
	    {
	        newTileDirection = direction;
	    }

	    public void ResizeTilemapTo(int width, int height)
	    {
	        if(width <= 0 || height <= 0)
	            throw new Exception("Values must be higher than 0.");
	        tilemap.SetSize(width, height);
	        CreateBarrier();
	        tilemap.GenerateEmptyTiles(currentLayer);
			markerManager.NewMapSize(width, height);
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
	      Pressed button to go to the next tile
	     */
	    public void NextTileButtonPressed()
	    {

	        if(currentTileName == null)
	        {
	            SelectTileType(previousTileType);
	            return;
	        }

	        TileType typeObject = null;
	        try
	        {
	            typeObject = tileTypeSet.GetTileTypeByName(currentTileName);
	        }
	        catch(EditorErrorException){ }

	        var currentIndex = tileTypeSet.types.IndexOf(typeObject);
	        int chosenIndex;

	        if(currentIndex+1 == tileTypeSet.types.Count)
	            chosenIndex = 0;
	        else
	            chosenIndex = currentIndex+1;

	        SelectTileType(tileTypeSet.types[chosenIndex].name);

	    }

	    /*
	      Pressed button to go to the previous tile
	     */
	    public void PreviousTileButtonPressed()
	    {

	        if(currentTileName == null)
	        {
	            SelectTileType(previousTileType);
	            return;
	        }

	        TileType typeObject = null;
	        try
	        {
	            typeObject = tileTypeSet.GetTileTypeByName(currentTileName);
	        }
	        catch(EditorErrorException){ }

	        var currentIndex = tileTypeSet.types.IndexOf(typeObject);
	        int chosenIndex;

	        if(currentIndex == 0)
	            chosenIndex = tileTypeSet.types.Count - 1;
	        else
	            chosenIndex = currentIndex-1;

	        SelectTileType(tileTypeSet.types[chosenIndex].name);

	    }

	    /*
	      Pressed button to cancel out the selection.
	     */
	    public void EmptyTileButtonPressed()
	    {
	        if(currentTileName == null)
	        {
	            SelectTileType(previousTileType);
	            return;
	        }
	        previousTileType = currentTileName;
	        SelectTileType(null);
	    }

	    /*
	      Edit tile button
	    */
	    public void EditTileButtonPressed()
	    {
	        if(currentTileName != null)
	            StartCoroutine(editTileDialog.Show(currentTileName));
	    }

		/*
	      Pressed button to go to the next tile marker
	     */
		public void NextTileMarkerButtonPressed()
		{
			if(selectedMarker == null)
			{
				SelectMarkerType(previousMarker == null ? null : previousMarker.name);
				return;
			}
			SelectMarkerType(markerManager.GetNextTileMarkerType(selectedMarker).name);
		}

		/*
	      Pressed button to go to the previous tile marker
	     */
		public void PreviousTileMarkerButtonPressed()
		{
			if(selectedMarker == null)
			{
				SelectMarkerType(previousMarker == null ? null : previousMarker.name);
				return;
			}
			SelectMarkerType(markerManager.GetPreviousTileMarkerType(selectedMarker).name);
		}

		/*
	      Pressed button to cancel out the marker selection.
	     */
		public void EmptyTileMarkerButtonPressed()
		{
			if(selectedMarker == null)
			{
				SelectMarkerType(previousMarker.name);
				return;
			}
			previousMarker = selectedMarker;
			SelectMarkerType(null);
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
		 * When switching to another major mode
		 */
		public void ChangeEditorModeButtonPressed()
		{
			var numModes = Enum.GetNames(typeof(EditorMode)).Length;
			SwitchEditorMode(
				(EditorMode)(((int)editorMode + 1) % numModes)
			);
		}

		/*
		 * Use to switch to a minor mode in objects
		 */
		public void SwitchWorldObjectMinorMode(int newMinorMode)
		{
			worldObjectMinorMode = newMinorMode;
			if(worldObjectMinorMode == 1)
				worldObjectMinorModeButton.GetComponentInChildren<Text>().text = "Moving";
			else
			{
				StopMovingWorldObject();
				worldObjectMinorModeButton.GetComponentInChildren<Text>().text = "Adding";
			}
		}

		public void StopMovingWorldObject()
		{
			if(worldObjectMoving != null)
			{
				var objRenderer = worldObjectMoving.gameObject.GetComponent<Renderer>();
				var matProp = new MaterialPropertyBlock();
				matProp.SetColor("_Color", new Color(1f, 1f, 1f));
				objRenderer.SetPropertyBlock(matProp);
			}
			worldObjectMoving = null;
		}

		public void WorldObjectMinorModeStartMovingWorldObject(WorldObject worldObject)
		{
			StopMovingWorldObject();
			var objRenderer = worldObject.gameObject.GetComponent<Renderer>();
			var matProp = new MaterialPropertyBlock();
			matProp.SetColor("_Color", new Color(0f, 1f, 1f));
			objRenderer.SetPropertyBlock(matProp);
			worldObjectMoving = worldObject;			
		}

		/*
		 * World object minor mode button pressed
		 */
		public void SwitchWorldObjectMinorModeButtonPressed()
		{
			SwitchWorldObjectMinorMode(Math.Abs(worldObjectMinorMode - 1));
		}

		/*
	      Pressed button to go to the next world object
	     */
		public void NextWorldObjectButtonPressed()
		{
			SelectWorldObjectType(worldObjectManager.GetNextWorldObjectMarkerType(selectedWorldObject).name);
		}

		/*
	      Pressed button to go to the previous world object
	     */
		public void PreviousWorldObjectButtonPressed()
		{
			SelectWorldObjectType(worldObjectManager.GetPreviousWorldObjectMarkerType(selectedWorldObject).name);
		}

		/*
	      Switches selected world object type
	     */
		private void SelectWorldObjectType(string worldObjectTypeName)
		{

			if(currentWorldObjectSelectedObj != null)
			{
				Destroy(currentWorldObjectSelectedObj);
				currentWorldObjectSelectedObj = null;
			}

			if(worldObjectTypeName == null)
			{
				currentWorldObjectText = null;
				currentWorldObjectText.text = "None";
				return;
			}

			selectedWorldObject = worldObjectManager.GetWorldObjectTypeByName(worldObjectTypeName);
			currentWorldObjectSelectedObj = Instantiate(selectedWorldObject.prefab);

			currentWorldObjectSelectedObj.transform.parent = currentWorldObjectPanel.transform;
			currentWorldObjectSelectedObj.AddComponent<RectTransform>();
			var rect = currentWorldObjectSelectedObj.GetComponent<RectTransform>();

			// Base values on template
			currentWorldObjectSelectedObj.layer = currentWorldObjectTemplate.layer;
			rect.localPosition = currentWorldObjectTemplate.GetComponent<RectTransform>().localPosition;
			rect.localScale = currentWorldObjectTemplate.GetComponent<RectTransform>().localScale;
			rect.localRotation = currentWorldObjectTemplate.GetComponent<RectTransform>().localRotation;

			currentWorldObjectText.text = selectedWorldObject.name;

		}

		/*
		 * Used when getting rid of a world object
		 */
		public void DeleteWorldObject(WorldObject worldObjectToDelete)
		{
			if(worldObjectToDelete == worldObjectMoving)
				StopMovingWorldObject();
			worldObjectManager.DeleteWorldObject(worldObjectToDelete);			
		}

	}

}