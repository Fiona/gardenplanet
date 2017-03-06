using System;
using System.IO;
using System.Collections.Generic;
using LitJson;
using System.Text;
using UnityEngine;

namespace StrawberryNova
{
		
	public class Map
	{

	    public struct MapTile
	    {
	        public int x;
	        public int y;
	        public int layer;
	        public string type;
	        public Direction direction;
	    }

	    public int width;
	    public int height;
	    public string filename;
	    public string fullFilepath;
	    public List<MapTile> tiles;

	    public Map()
	    {
	        tiles = new List<MapTile>();

	    }

	    public Map(string filename)
	    {
	        this.filename = filename;
	        width = 5;
	        height = 5;
	        tiles = new List<MapTile>();

	        if(filename == null)
	            return;

	        fullFilepath = GetMapFilePathFromName(filename);

	        // Load it
	        if(File.Exists(fullFilepath))
	        {
	            Map loadedMap;
	            try
	            {
	                using(var fh = File.OpenText(fullFilepath))
	                    loadedMap = JsonMapper.ToObject<Map>(fh.ReadToEnd());
	            }
	            catch(JsonException e)
	            {
	                Debug.Log(e);
	                throw new EditorErrorException("Error loading map.");
	            }
	            this.tiles = loadedMap.tiles;
	            this.width = loadedMap.width;
	            this.height = loadedMap.height;
	        }

	    }

	    // Constructor for converting from tilemap
	    public Map(string filename, Tilemap tilemap)
	    {
	        this.filename = filename;
	        fullFilepath = GetMapFilePathFromName(filename);
	        width = tilemap.width;
	        height = tilemap.height;

	        tiles = new List<MapTile>();
	        foreach(var tile in tilemap.tilemap)
	        {
	            if(tile.emptyTile)
	                continue;
	            var newTile = new MapTile(){
	                x=tile.x,
	                y=tile.y,
	                layer=tile.layer,
	                direction=tile.direction,
	                type=tile.tileTypeName
	            };
	            tiles.Add(newTile);
	        }
	    }

	    public void SaveMap()
	    {
	        string jsonOutput;
	        try
	        {
	            jsonOutput = JsonMapper.ToJson(this);
	        }
	        catch(JsonException e)
	        {
	            Debug.Log(e);
	            throw new EditorErrorException("Error saving map.");
	        }

	        // Check directories exist
	        if(!Directory.Exists(Consts.DATA_DIR))
	            Directory.CreateDirectory(Consts.DATA_DIR);
	        if(!Directory.Exists(Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_MAPS)))
	            Directory.CreateDirectory(Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_MAPS));

	        using(var fh = File.OpenWrite(fullFilepath))
	        {
	            var jsonBytes = Encoding.UTF8.GetBytes(jsonOutput);
	            fh.SetLength(0);
	            fh.Write(jsonBytes, 0, jsonBytes.Length);
	        }
	    }

	    public static string GetMapFilePathFromName(string name)
	    {
	        return Path.Combine(
	            Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_MAPS),
	            String.Format("{0}.{1}", name, Consts.FILE_EXTENSION_MAP)
	            );
	    }

	    public static bool DoesMapNameExist(string name)
	    {
	        var path = Map.GetMapFilePathFromName(name);
	        return File.Exists(path);
	    }

	}

}