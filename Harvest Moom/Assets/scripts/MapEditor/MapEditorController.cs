using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class MapEditorController : MonoBehaviour
{

    public int currentLayer;

    [Header("Editor component references")]
    public Tilemap tilemap;
    public Camera camera;
    public Text layerText;
    public GameObject currentTilePanel;
    public Text currentTileText;
    public GameObject TilemapSelectionCube;
    public GameObject barrierTemplate;

    private string currentTileName;
    private GameObject currentTileTypeSelectedObj;
    private Tilemap.Tile currentHoveredTile;
    private List<GameObject> barriers;

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
                    tilemap.AddTile(((UnityEngine.Random.Range(0.0f, 1.0f) > .5f) ? "grass02" : "grass01"), x, y, 0);
                //else
                //    tilemap.AddTile(null, x, y, 0);
            }
        }

        // Set up editor
        SwitchToLayer(0);
        var tileTypes = tilemap.GetTileTypes();
        SelectTileType(tileTypes.GetKeyFromIndex(0));

    }

    public void Update()
    {
        // Do bump scrolling
        var body = camera.GetComponent<Rigidbody>();
        Vector3? direction = null;

        if(Input.mousePosition[0] < Consts.MOUSE_BUMP_BORDER)
            direction = Vector3.left;
        if(Input.mousePosition[0] > Screen.width - Consts.MOUSE_BUMP_BORDER)
            direction = Vector3.right;
        if(Input.mousePosition[1] < Consts.MOUSE_BUMP_BORDER)
            direction = Vector3.back;
        if(Input.mousePosition[1] > Screen.height - Consts.MOUSE_BUMP_BORDER)
            direction = Vector3.forward;

        if(direction != null)
            body.AddForce(((Vector3)direction) * Consts.MOUSE_BUMP_SPEED * Time.deltaTime);

        // Clamp camera pos
        camera.transform.position = new Vector3(
            Mathf.Clamp(camera.transform.position.x, 0, tilemap.width),
            camera.transform.position.y,
            Mathf.Clamp(camera.transform.position.z, -Consts.VERTICAL_EDGE_DISTANCE, tilemap.height - Consts.VERTICAL_EDGE_DISTANCE)
            );
    }

    public void ClickedOnWorld()
    {
        if(currentHoveredTile == null)
            return;

        int x = currentHoveredTile.x;
        int y = currentHoveredTile.y;
        int layer = currentHoveredTile.layer;

        tilemap.RemoveTile(x, y, layer);
        tilemap.AddTile(currentTileName, x, y, layer);
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
      Called from Tilemap when the mouse is over a tile
     */
    public void SelectedNewTile(Tilemap.Tile selectedTile)
    {
        currentHoveredTile = selectedTile;

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
        camera.transform.position = new Vector3(
            camera.transform.position.x,
            y,
            camera.transform.position.z);
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

        // Position according to magic values subject to change
        currentTileTypeSelectedObj.transform.localPosition = new Vector3(105.0f, 23.0f, -243.48f);
        currentTileTypeSelectedObj.transform.localScale = new Vector3(10000.0f, 10000.0f, 10000.0f);
        currentTileTypeSelectedObj.transform.localRotation = Quaternion.Euler(32.0f, 120.0f, 0.0f);

        // Switch to special ordered shader
        var fadeOverShader = Shader.Find("Custom/FadeOver");
        var renderer = currentTileTypeSelectedObj.GetComponent<Renderer>();
        for(var i = 0; i < renderer.materials.Length; i++)
            renderer.materials[i].shader = fadeOverShader;

        currentTileName = tileTypeName;
        currentTileText.text = currentTileName;

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
            tilemap.width / 2,
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
            tilemap.width / 2,
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
            (tilemap.height/2) - 0.5f
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
            (tilemap.height/2) - 0.5f
            );
        rightBarrier.transform.Rotate(new Vector3(0f, 0f, -90f));
        barriers.Add(rightBarrier);

    }

}
