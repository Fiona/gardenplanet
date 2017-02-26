using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapEditorController : MonoBehaviour
{

    public int currentLayer;

    [Header("Editor component references")]
    public Tilemap tilemap;
    public Camera mainCamera;
    public Text layerText;
    public GameObject currentTilePanel;
    public Text currentTileText;
    public GameObject currentTileTemplate;
    public GameObject TilemapSelectionCube;
    public GameObject barrierTemplate;
    public Image worldPanel;
    public MainMenuBar mainMenuBar;
    public YesNoDialog yesNoDialog;

    private string currentTileName;
    private GameObject currentTileTypeSelectedObj;
    private Tilemap.Tile currentHoveredTile;
    private List<GameObject> barriers;
    private Direction newTileDirection;

    public void Awake()
    {

        barriers = new List<GameObject>();

        var tileMapWidth = 15;
        var tileMapHeight = 10;
        tilemap.SetSize(tileMapWidth, tileMapHeight);

        for(int x = 0; x < tileMapWidth; x++)
        {
            for(int y = 0; y < tileMapHeight; y++)
            {
                if(x < 10 && y < 10)
                    tilemap.AddTile(((UnityEngine.Random.Range(0.0f, 1.0f) > .5f) ? "grass02" : "grass01"), x, y, 0, Direction.Down);
                //else
                //    tilemap.AddTile(null, x, y, 0);
            }
        }

        // Set up editor
        SetNewTileDirection(Direction.Down);
        currentTileTemplate.SetActive(false);
        SwitchToLayer(0);
        var tileTypes = tilemap.GetTileTypes();
        SelectTileType(tileTypes.GetKeyFromIndex(0));

    }

    public void Update()
    {
        // Do bump scrolling
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

        ClampCameraToBorders();
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

            tilemap.RemoveTile(x, y, layer);
            tilemap.AddTile(currentTileName, x, y, layer, newTileDirection);
        }
        else if(pointerEventData.button == PointerEventData.InputButton.Right)
            SelectTileType(currentHoveredTile.tileTypeName);
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

        var tileTypes = tilemap.GetTileTypes();
        if(!tileTypes.ContainsKey(tileTypeName))
            throw new Exception("Tile attempted to change to not in tile type dictionary.");

        if(currentTileTypeSelectedObj != null)
        {
            Destroy(currentTileTypeSelectedObj);
            currentTileTypeSelectedObj = null;
        }

        currentTileTypeSelectedObj = Instantiate(tileTypes[tileTypeName]) as GameObject;
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

        var tileTypes = tilemap.GetTileTypes();
        var currentIndex = tileTypes.IndexOf(currentTileName);
        int chosenIndex;

        if(currentIndex+1 == tileTypes.Count)
            chosenIndex = 0;
        else
            chosenIndex = currentIndex+1;

        SelectTileType(tileTypes.GetKeyFromIndex(chosenIndex));

    }

    /*
      Pressed button to go to the previous tile
     */
    public void PreviousTileButtonPressed()
    {

        var tileTypes = tilemap.GetTileTypes();
        var currentIndex = tileTypes.IndexOf(currentTileName);
        int chosenIndex;

        if(currentIndex == 0)
            chosenIndex = tileTypes.Count - 1;
        else
            chosenIndex = currentIndex-1;

        SelectTileType(tileTypes.GetKeyFromIndex(chosenIndex));

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

}
