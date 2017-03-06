using UnityEngine;
using System;
using System.Collections.Generic;

namespace StrawberryNova
{

	public class Tilemap : MonoBehaviour
	{

	    public class Tile
	    {
	        public int x;
	        public int y;
	        public int layer;
	        public Direction direction;
	        public GameObject tileObj;
	        public bool emptyTile;
	        public string tileTypeName;
	        public Material[] sharedMaterials;

	        private static PhysicMaterial wallPhysicMaterial;

	        public Tile(int x, int y, int layer, Direction direction, GameObject tileObj, string tileTypeName)
	        {
	            if(Tile.wallPhysicMaterial == null)
	                Tile.wallPhysicMaterial = (PhysicMaterial)Resources.Load("WallPhysicMaterial") as PhysicMaterial;
	            // Place tile
	            this.x = x;
	            this.y = y;
	            this.layer = layer;
	            this.tileObj = tileObj;
	            this.tileTypeName = tileTypeName;
	            this.emptyTile = (tileTypeName == null);

	            tileObj.transform.localPosition = new Vector3(
	                x,
	                layer * Consts.TILE_HEIGHT,
	                y);

	            SetDirection(direction);
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

	            // Editor tile floor
	            if(FindObjectOfType<App>().state == AppState.Editor)
	            {
	                BoxCollider floorCollider = tileObj.AddComponent<BoxCollider>();
	                floorCollider.size = new Vector3(0.01f, 0.001f, 0.01f);
	                floorCollider.center = new Vector3(0.0f, -0.0004f, 0.0f);
	                return;
	            }

	            var tileType = FindObjectOfType<GameController>().tileTypeSet.GetTileTypeByName(tileTypeName);

	            // For each volume, create them if they're a recognisable
	            // collision shape.
	            foreach(var volume in tileType.volumes)
	            {
	                BoxCollider newCollider = tileObj.AddComponent<BoxCollider>();
	                float yScale;
	                if(volume.type == TileTypeVolumeType.CollisionPlane)
	                    yScale = 0.0005f;
	                else
	                    yScale = (Consts.VOLUME_SCALE_DEFAULT / 100.0f) * volume.yScale;
	                newCollider.size = new Vector3(
	                    (Consts.VOLUME_SCALE_DEFAULT / 100.0f) * volume.xScale,
	                    yScale,
	                    (Consts.VOLUME_SCALE_DEFAULT / 100.0f) * volume.zScale
	                    );
	                newCollider.center = new Vector3(
	                    Consts.VOLUME_POSITION_SHIFT_PER_UNIT * (float)volume.x,
	                    Consts.VOLUME_POSITION_SHIFT_PER_UNIT * (float)volume.y,
	                    Consts.VOLUME_POSITION_SHIFT_PER_UNIT * (float)volume.z
	                    );
	                if(volume.type == TileTypeVolumeType.CollisionPlane)
	                    newCollider.center += new Vector3(0.0f, -0.0002f, 0.0f);
	                if(volume.isWall)
	                    newCollider.material = Tile.wallPhysicMaterial;
	            }

	        }

	        public void SetDirection(Direction direction)
	        {
	            this.direction = direction;
	            var baseRotation = 180;
	            if(direction == Direction.Right)
	                baseRotation -= 90;
	            if(direction == Direction.Up)
	                baseRotation -= 180;
	            if(direction == Direction.Left)
	                baseRotation += 90;
	            tileObj.transform.localRotation = Quaternion.Euler(0, baseRotation, 0);
	        }
	    }

	    private TileTypeSet tileTypeSet;
	    private Tile currentTileMouseOver = null;

	    [HideInInspector]
	    public MapEditorController mapEditor;
	    [HideInInspector]
	    public int width;
	    [HideInInspector]
	    public int height;
	    [HideInInspector]
	    public List<Tile> tilemap;


	    /*
	      Loads in all the available tile types
	     */
	    public void Awake()
	    {
	        mapEditor = FindObjectOfType<MapEditorController>();
	        tilemap = new List<Tile>();
	    }

	    /*
	      Initialises the tilemap using the passed Map object.
	    */
	    public void LoadFromMap(Map map, TileTypeSet tileTypeSet)
	    {

	        this.tileTypeSet = tileTypeSet;

	        // Destroy old one
	        if(tilemap.Count > 0)
	        {
	            var tilemapClone = new List<Tile>(tilemap);
	            foreach(var tile in tilemapClone)
	                RemoveTile(tile.x, tile.y, tile.layer);
	            tilemap = new List<Tile>();
	        }

	        // Load in tiles
	        SetSize(map.width, map.height);
	        foreach(var tile in map.tiles)
	            AddTile(tile.type, tile.x, tile.y, tile.layer, tile.direction);
	    }

	    /*
	      The tilemap will be of 0 size if this hasn't been called. Also
	      kills all orphaned tiles.
	     */
	    public void SetSize(int width, int height)
	    {
	        this.width = width;
	        this.height = height;

	        var tilesToKill = new List<Tile>();
	        foreach(var tile in tilemap)
	            if(tile.x >= width || tile.y >= height)
	                tilesToKill.Add(tile);
	        foreach(var tile in tilesToKill)
	            RemoveTile(tile.x, tile.y, tile.layer);
	    }

	    /*
	      Helper method fer adding a tile including it's gameobject at
	      a specified tile position.
	     */
	    public void AddTile(string tilename, int x, int y, int layer, Direction direction)
	    {

	        if(x < 0 || x >= width || y < 0 || y >= height)
	        {
	            Debug.Log("Tile outside of tilemap.");
	            return;
	        }

	        var newTileObject = tileTypeSet.InstantiateTile(tilename);
	        newTileObject.transform.parent = transform;

	        var newTile = new Tile(x, y, layer, direction, newTileObject, tilename);
	        tilemap.Add(newTile);

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
	      Pass a Tile object to tell the tilemap that we're hovering
	      over the tile that object represents.
	     */
	    public void MouseOverTile(Tile tile)
	    {
	        if(mapEditor != null)
	        {

	            // Make older hovered tile visible
	            if(currentTileMouseOver != tile && currentTileMouseOver != null && currentTileMouseOver.emptyTile == false)
	            {
	                // Destroy the old instanced materials and go back to the shared ones
	                if(currentTileMouseOver.tileObj != null)
	                {
	                    var tileRenderer = currentTileMouseOver.tileObj.GetComponent<Renderer>();
	                    for(var i = 0; i < tileRenderer.materials.Length; i++)
	                        Destroy(tileRenderer.materials[i]);
	                    tileRenderer.materials = currentTileMouseOver.sharedMaterials;
	                }
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
	        if(tileObject == null || tilemap == null)
	            return null;
	        foreach(var tile in tilemap)
	        {
	            if(tile.tileObj == tileObject)
	                return tile;
	        }
	        return null;
	    }

	    /*
	      Used in the editor when switching layers to create fake ass
	      empty tiles for capturing rays.
	     */
	    public void GenerateEmptyTiles(int layer)
	    {

	        if(tilemap == null)
	            return;

	        var clonedTilemap = new List<Tile>(tilemap);
	        foreach(var tile in clonedTilemap)
	            if(tile.emptyTile)
	                RemoveTile(tile.x, tile.y, tile.layer);

	        for(int x = 0; x < width; x++)
	            for(int y = 0; y < height; y++)
	                if(GetTileAt(x, y, layer) == null)
	                    AddTile(null, x, y, layer, Direction.Down);

	    }

	    /*
	      Pass a tile object to rotate it 90 degrees in the rotational
	      direction passed.
	     */
	    public void RotateTileInDirection(Tile tile, RotationalDirection direction)
	    {
	        var newDirection = Direction.Down;
	        if(direction == RotationalDirection.AntiClockwise)
	        {
	            switch(tile.direction)
	            {
	                case Direction.Down:
	                    newDirection = Direction.Left;
	                    break;
	                case Direction.Left:
	                    newDirection = Direction.Up;
	                    break;
	                case Direction.Up:
	                    newDirection = Direction.Right;
	                    break;
	                case Direction.Right:
	                    newDirection = Direction.Down;
	                    break;
	            }
	        }
	        else if(direction == RotationalDirection.Clockwise)
	        {
	            switch(tile.direction)
	            {
	                case Direction.Down:
	                    newDirection = Direction.Right;
	                    break;
	                case Direction.Left:
	                    newDirection = Direction.Down;
	                    break;
	                case Direction.Up:
	                    newDirection = Direction.Left;
	                    break;
	                case Direction.Right:
	                    newDirection = Direction.Up;
	                    break;
	            }
	        }
	        tile.SetDirection(newDirection);
	    }

	}

}