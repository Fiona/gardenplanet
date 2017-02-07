using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Tilemap : MonoBehaviour
{

    public class Tile
    {
        public int x;
        public int y;
        public int z;
        public GameObject tileObj;

        public Tile(int _x, int _y, int _z, GameObject _tileObj)
        {
            // Place tile
            x = _x; y = _y; z = _z; tileObj = _tileObj;
            tileObj.transform.localPosition = new Vector3(
                x,
                _z * Consts.TILE_HEIGHT,
                y);
            tileObj.transform.Rotate(new Vector3(0, 180, 0));
            // Combats tile gaps
            tileObj.transform.localScale = new Vector3(100.0f + Consts.SCALE_FUDGE, 100.0f + Consts.SCALE_FUDGE, 100.0f + Consts.SCALE_FUDGE);

            CreateCollisionShapes();
        }

        public void CreateCollisionShapes()
        {
            // Set collision layer
            tileObj.layer = Consts.COLLISION_LAYER_TILES;
            // Floor
            BoxCollider floorCollider = tileObj.AddComponent<BoxCollider>();
            floorCollider.size = new Vector3(0.01f, 0.001f, 0.01f);
            floorCollider.center = new Vector3(0.0f, -0.0004f, 0.0f);
        }
    }

    private Dictionary<string, GameObject> tileTypes;
    private List<Tile> tilemap;
    private Tile currentTileMouseOver = null;

    public MapEditorController mapEditor;

    /*
      Constructor
    */
    public Tilemap()
    {
        tilemap = new List<Tile>();
    }

    /*
      Loads in all the available tile types
     */
    public void Awake()
    {
        tileTypes = new Dictionary<string, GameObject>();
        var newTileTypes = Resources.LoadAll("tiles/");
        foreach(var newType in newTileTypes)
            tileTypes[newType.name] = (GameObject)newType;
    }

    /*
      Helper method fer adding a tile including it's gameobject at
      a specified tile position.
     */
    public void addTile(string tilename, int x, int y, int z)
    {
        GameObject newTileObj = Instantiate(
                Resources.Load("tiles/" + tilename, typeof(GameObject))
            ) as GameObject;
        newTileObj.transform.parent = transform;
        var newTile = new Tile(x, y, z, newTileObj);
        tilemap.Add(newTile);
    }

    /*
      Pass a GameObject to tell the tilemap that we're hovering
      over the tile that object represents.
     */
    public void mouseOverTile(GameObject tileGameObject)
    {
        var tile = getTileFromGameObject(tileGameObject);

        if(currentTileMouseOver == tile)
            return;

        // Make older hovered tile visible
        if(currentTileMouseOver != null)
        {
            foreach(var mat in currentTileMouseOver.tileObj.GetComponent<Renderer>().materials)
                mat.color = new Color(mat.color[0], mat.color[1], mat.color[2], 1.0f);
        }

        currentTileMouseOver = tile;

        if(currentTileMouseOver == null || tile.z != mapEditor.currentLayer)
        {
            currentTileMouseOver = null;
            return;
        }

        // Make new tile hovered visible
        foreach(var mat in tile.tileObj.GetComponent<Renderer>().materials)
            mat.color = new Color(mat.color[0], mat.color[1], mat.color[2], 0.6f);
    }

    /*
      Returns a reference to the Tile object in the tilemap list
      that corresponds to the passed GameObject. Used for collisions.
     */
    public Tile getTileFromGameObject(GameObject tileObject)
    {
        foreach(var tile in tilemap)
        {
            if(tile.tileObj == tileObject)
                return tile;
        }
        return null;
    }

}
