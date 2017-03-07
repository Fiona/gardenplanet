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

		[Header("Object mode")]
		public GameObject modeObject;

	    [HideInInspector]
	    public TileTypeSet tileTypeSet;
		[HideInInspector]
		public EditorMode editorMode;

	    string currentTileName;
	    GameObject currentTileTypeSelectedObj;
	    string previousTileType;
	    Tilemap.Tile currentHoveredTile;
	    List<GameObject> barriers;
	    Direction newTileDirection;
		MarkerManager markerManager;
		TileMarkerType selectedMarker;
		TileMarkerType previousMarker;

	    public void Awake()
	    {
			var markerManagerObj = new GameObject("MarkerManager");
			markerManager = markerManagerObj.AddComponent<MarkerManager>();

	        barriers = new List<GameObject>();
	        try
	        {
	            tileTypeSet = new TileTypeSet("default");
	            LoadMap(null);
	        }
	        catch(EditorErrorException){}

	        SwitchEditorMode(EditorMode.Tile);
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
	    }
			
	    public void SwitchEditorMode(EditorMode editorMode)
	    {
	        this.editorMode = editorMode;
			modeObject.SetActive(false);
			modeTile.SetActive(false);
			modeMarking.SetActive(false);
			switch(this.editorMode)
			{
				case EditorMode.Object:
					modeObject.SetActive(true);
					this.currentModeText.text = "Object Mode";
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
	        SetNewTileDirection(Direction.Down);
	        currentTileTemplate.SetActive(false);
	        SwitchToLayer(0);
	        SelectTileType(tileTypeSet.types[0].name);
			SelectMarkerType(markerManager.tileMarkerTypes[0].name);
	        mainMenuBar.ShowGoodMessage("Loaded map");
	    }

	    // Saves the current Tilemap to the current Map file.
	    public void SaveMap()
	    {
	        try
	        {
				map = new Map(map.filename, tilemap, markerManager);
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

			if(pointerEventData.button == PointerEventData.InputButton.Left)
			{
				int x = currentHoveredTile.x;
				int y = currentHoveredTile.y;
				int layer = currentHoveredTile.layer;

				if(editorMode == EditorMode.Tile)
				{
					try
					{
						tilemap.RemoveTile(x, y, layer);
						tilemap.AddTile(currentTileName, x, y, layer, newTileDirection);
					}
					catch(EditorErrorException)
					{
					}
				}
				else if(editorMode == EditorMode.Marker)
				{
					try
					{
						markerManager.RemoveMarkerAt(x, y, layer);
						markerManager.AddMarkerAt(selectedMarker, x, y, layer, newTileDirection);
					}
					catch(EditorErrorException)
					{
					}
				}
			}
			else if(pointerEventData.button == PointerEventData.InputButton.Right)
			{
				if(editorMode == EditorMode.Tile)
					SelectTileType(currentHoveredTile.tileTypeName);
				else if(editorMode == EditorMode.Marker)
				{
					var marker = markerManager.GetMarkerAt(
						currentHoveredTile.x, currentHoveredTile.y, currentHoveredTile.layer
					);
					SelectMarkerType(marker == null ? null : marker.name);
				}
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

	        if(currentHoveredTile == null)
	            TilemapSelectionCube.GetComponent<MeshRenderer>().enabled = false;
	        else
	        {
	            TilemapSelectionCube.GetComponent<MeshRenderer>().enabled = true;
	            TilemapSelectionCube.transform.localPosition = new Vector3(
	                currentHoveredTile.x,
	                0.3f + (currentHoveredTile.layer * Consts.TILE_HEIGHT),
	                currentHoveredTile.y);
	        }
	    }

	    /*
	      Switches to the specified map layer
	     */
	    private void SwitchToLayer(int layer)
	    {
	        currentLayer = layer;
	        layerText.text = String.Format("{0}", layer);
	        float y = Consts.CAMERA_Y + (Consts.TILE_HEIGHT * layer);
	        mainCamera.transform.position = new Vector3(
	            mainCamera.transform.position.x,
	            y,
	            mainCamera.transform.position.z);
	        CreateBarrier();
	        tilemap.GenerateEmptyTiles(currentLayer);
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
	            0.1f * tilemap.width,
	            topBarrier.transform.localScale.y,
	            0.02f
	            );
	        topBarrier.transform.localPosition = new Vector3(
	            (tilemap.width / 2.0f) - 0.5f,
	            0.098f + (currentLayer * 0.5f),
	            tilemap.height - 0.5f
	            );
	        barriers.Add(topBarrier);

	        // Bottom of barrier
	        var bottomBarrier = Instantiate(barrierTemplate);
	        bottomBarrier.transform.parent = tilemap.transform;
	        bottomBarrier.transform.localScale = new Vector3(
	            0.1f * tilemap.width,
	            bottomBarrier.transform.localScale.y,
	            0.02f
	            );
	        bottomBarrier.transform.localPosition = new Vector3(
	            (tilemap.width / 2.0f) - 0.5f,
	            0.098f + (currentLayer * 0.5f),
	            -0.5f
	            );
	        barriers.Add(bottomBarrier);

	        // Left barrier
	        var leftBarrier = Instantiate(barrierTemplate);
	        leftBarrier.transform.parent = tilemap.transform;
	        leftBarrier.transform.localScale = new Vector3(
	            0.1f * tilemap.height,
	            leftBarrier.transform.localScale.y,
	            0.02f
	            );
	        leftBarrier.transform.localPosition = new Vector3(
	            -0.5f,
	            0.098f + (currentLayer * 0.5f),
	            (tilemap.height / 2.0f) - 0.5f
	            );
	        leftBarrier.transform.Rotate(new Vector3(0f, 0f, 90f));
	        barriers.Add(leftBarrier);

	        // Riiight
	        var rightBarrier = Instantiate(barrierTemplate);
	        rightBarrier.transform.parent = tilemap.transform;
	        rightBarrier.transform.localScale = new Vector3(
	            0.1f * tilemap.height,
	            rightBarrier.transform.localScale.y,
	            0.02f
	            );
	        rightBarrier.transform.localPosition = new Vector3(
	            tilemap.width-0.5f,
	            0.098f + (currentLayer * 0.5f),
	            (tilemap.height / 2.0f) - 0.5f
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

		public void ChangeEditorModeButtonPressed()
		{
			var numModes = Enum.GetNames(typeof(EditorMode)).Length;
			SwitchEditorMode(
				(EditorMode)(((int)editorMode + 1) % numModes)
			);
		}

	}

}