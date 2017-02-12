using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Tilemap : MonoBehaviour
{

    public class Tile
    {
        public int x;
        public int y;
        public int layer;
        public GameObject tileObj;
        public bool emptyTile;
        public Material[] sharedMaterials;

        public Tile(int _x, int _y, int _layer, GameObject _tileObj, bool _emptyTile=false)
        {
            // Place tile
            x = _x; y = _y; layer = _layer; tileObj = _tileObj;
            emptyTile = _emptyTile;
            tileObj.transform.localPosition = new Vector3(
                x,
                _layer * Consts.TILE_HEIGHT,
                y);

            tileObj.transform.Rotate(new Vector3(0, 180, 0));
            // Combats tile gaps
            tileObj.transform.localScale = new Vector3(100.0f + Consts.SCALE_FUDGE, 100.0f + Consts.SCALE_FUDGE, 100.0f + Consts.SCALE_FUDGE);

            if(!emptyTile)
                sharedMaterials = tileObj.GetComponent<Renderer>().sharedMaterials;

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

    private TileTypesDictionary tileTypes;
    private List<Tile> tilemap;
    private Tile currentTileMouseOver = null;
    private float[] tileExtents = new float[4];

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
        tileTypes = new TileTypesDictionary();
        var unsortedNewTileTypes = Resources.LoadAll("tiles/");

        GameObject[] newTileTypes = unsortedNewTileTypes.Select(r => (r as GameObject)).
            Where(r => r != null).
            OrderBy(t => t.name).
            ToArray<GameObject>();

        foreach(var newType in newTileTypes)
            tileTypes[newType.name] = (GameObject)newType;
    }

    /*
      Helper method fer adding a tile including it's gameobject at
      a specified tile position.
     */
    public void AddTile(string tilename, int x, int y, int layer)
    {
        // If we want a named tile or an empty one
        GameObject newTileObj = null;
        if(tilename != null)
            newTileObj = Instantiate(tileTypes[tilename]) as GameObject;
        else
            newTileObj = new GameObject("Empty Tile");
        newTileObj.transform.parent = transform;

        var newTile = new Tile(x, y, layer, newTileObj, (tilename==null));
        tilemap.Add(newTile);

        // Find new extents
        float? minZ = null, maxZ = null, minX = null, maxX = null;
        foreach(var tile in tilemap)
        {
            float tileX = tile.tileObj.transform.localPosition.x;
            float tileZ = tile.tileObj.transform.localPosition.z;
            if(minX == null || tileX < minX)
                minX = tileX;
            if(maxX == null || tileX > maxX)
                maxX = tileX;
            if(minZ == null || tileZ < minZ)
                minZ = tileZ;
            if(maxZ == null || tileZ > maxZ)
                maxZ = tileZ;
        }
        tileExtents[0] = (float)minX; tileExtents[1] = (float)maxX;
        tileExtents[2] = (float)minZ; tileExtents[3] = (float)maxZ;
    }

    /*
      If passed an X, Y and layer the tile at that position will be removed.
     */
    public void RemoveTile(int x, int y, int layer)
    {

        Tile tileToRemove = GetTileAt(x, y, layer);

        if(tileToRemove != null)
        {
            if(!tileToRemove.emptyTile)
            {
                // Check materials need cleaning
                var renderer = tileToRemove.tileObj.GetComponent<Renderer>();

                if(renderer.materials != renderer.sharedMaterials)
                    for(var i = 0; i < renderer.materials.Length; i++)
                        Destroy(renderer.materials[i]);
            }

            // Delete game object and Tile instance
            Destroy(tileToRemove.tileObj);
            tilemap.Remove(tileToRemove);
        }

    }

    /*
      Pass a GameObject to tell the tilemap that we're hovering
      over the tile that object represents.
     */
    public void MouseOverTile(GameObject tileGameObject)
    {
        var tile = GetTileFromGameObject(tileGameObject);

        if(currentTileMouseOver == tile)
            return;

        if(mapEditor != null)
        {

            // Make older hovered tile visible
            if(currentTileMouseOver != tile && currentTileMouseOver != null && currentTileMouseOver.emptyTile == false)
            {
                // Destroy the old instanced materials and go back to the shared ones
                var tileRenderer = currentTileMouseOver.tileObj.GetComponent<Renderer>();
                for(var i = 0; i < tileRenderer.materials.Length; i++)
                    Destroy(tileRenderer.materials[i]);
                tileRenderer.materials = currentTileMouseOver.sharedMaterials;
            }

        }

        if(mapEditor != null && tile != null && tile.layer != mapEditor.currentLayer)
            currentTileMouseOver = null;
        else
            currentTileMouseOver = tile;

        // Make new tile hovered slightly transparent in editor
        if(mapEditor != null)
        {
            if(currentTileMouseOver != null && currentTileMouseOver != null && currentTileMouseOver.emptyTile == false)
                foreach(var mat in currentTileMouseOver.tileObj.GetComponent<Renderer>().materials)
                    mat.color = new Color(mat.color[0], mat.color[1], mat.color[2], 0.8f);
            mapEditor.SelectedNewTile(currentTileMouseOver);
        }
    }

    /*
      Returns a reference to the Tile object at the position requested.
      Null if no tile exists there.
     */
    public Tile GetTileAt(int x, int y, int layer)
    {
        foreach(var tile in tilemap)
        {
            if(tile.x == x && tile.y == y && tile.layer == layer)
                return tile;
        }
        return null;
    }

    /*
      Returns a reference to the Tile object in the tilemap list
      that corresponds to the passed GameObject. Used for collisions.
     */
    public Tile GetTileFromGameObject(GameObject tileObject)
    {
        if(tileObject == null)
            return null;
        foreach(var tile in tilemap)
        {
            if(tile.tileObj == tileObject)
                return tile;
        }
        return null;
    }

    /*
      Returns 4 item array containing the bounds of tiles at the edges of the world.
      [minX, maxX, minZ, maxZ]
     */
    public float[] GetTileExtents()
    {
        return tileExtents;
    }

    /*
      Returns dictionary relating to tile types
    */
    public TileTypesDictionary GetTileTypes()
    {
        return tileTypes;
    }

}
