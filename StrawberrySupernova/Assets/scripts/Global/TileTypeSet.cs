using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using System.Text;
using UnityEngine;

public class TileTypeSet
{

    public string name;
    public string fullFilePath;
    public List<TileType> types;

    private Dictionary<string, GameObject> tileGameObjects;

    public TileTypeSet()
    {
        types = new List<TileType>();
        tileGameObjects = new Dictionary<string, GameObject>();
    }

    public TileTypeSet(string name)
    {
        this.name = name;
        fullFilePath = GetFilePathFromName(name);
        types = new List<TileType>();

        // Load in existing set
        if(File.Exists(fullFilePath))
        {
            TileTypeSet loadedSet;
            try
            {
                using(var fh = File.OpenText(fullFilePath))
                    loadedSet = JsonMapper.ToObject<TileTypeSet>(fh.ReadToEnd());
            }
            catch(JsonException e)
            {
                Debug.Log(e);
                throw new EditorErrorException("Error loading set.");
            }
            this.name = loadedSet.name;
            this.types = loadedSet.types;
        }

        var resaveTileSet = false;

        // Update with new tiles
        var allTileTypes = Resources.LoadAll("tiles/");
        bool inSet;
        foreach(var filesystemType in allTileTypes)
        {
            inSet = false;
            foreach(var loadedType in this.types)
            {
                if(filesystemType.name == loadedType.name)
                {
                    inSet = true;
                    break;
                }
            }
            if(!inSet)
            {
                var newTileType = new TileType
                {
                    name=filesystemType.name,
                    volumes=new List<TileTypeVolume>()
                };
                this.types.Add(newTileType);
                resaveTileSet = true;
            }
        }

        // Delete removed tiles from the set
        bool inFilesystem;
        var clonedTypes = new List<TileType>(this.types);
        foreach(var loadedType in clonedTypes)
        {
            inFilesystem = false;
            foreach(var filesystemType in allTileTypes)
            {
                if(filesystemType.name == loadedType.name)
                {
                    inFilesystem = true;
                    break;
                }
            }
            if(!inFilesystem)
            {
                this.types.Remove(loadedType);
                resaveTileSet = true;
            }
        }

        // Reorder tile types and resave file
        if(resaveTileSet)
        {
            this.types.Sort((x, y) => x.name.CompareTo(y.name));
            SaveTileTypeSet();
        }

        // Load game objects
        tileGameObjects = new Dictionary<string, GameObject>();
        foreach(var tileType in allTileTypes)
            tileGameObjects[tileType.name] = (GameObject)tileType;

    }

    public void SaveTileTypeSet()
    {
        string jsonOutput;
        try
        {
            jsonOutput = JsonMapper.ToJson(this);
        }
        catch(JsonException e)
        {
            Debug.Log(e);
            throw new EditorErrorException("Error saving tile set.");
        }

        // Check directories exist
        if(!Directory.Exists(Consts.DATA_DIR))
            Directory.CreateDirectory(Consts.DATA_DIR);

        using(var fh = File.OpenWrite(fullFilePath))
        {
            var jsonBytes = Encoding.UTF8.GetBytes(jsonOutput);
            fh.SetLength(0);
            fh.Write(jsonBytes, 0, jsonBytes.Length);
        }
    }

    public static string GetFilePathFromName(string name)
    {
        return Path.Combine(
            Consts.DATA_DIR,
            String.Format("{0}.{1}", name, Consts.FILE_EXTENSION_TILE_SET)
            );
    }

    public TileType GetTileTypeByName(string name)
    {
        if(name == null)
            return null;
        foreach(var type in this.types)
            if(type.name == name)
                return type;
        throw new EditorErrorException("Couldn't find tile type requested.");
    }

    public GameObject InstantiateTile(TileType tileName)
    {
        return InstantiateTile(tileName.name);
    }

    public GameObject InstantiateTile(string tileName)
    {
        GameObject newTileObj = null;
        if(tileName != null)
            newTileObj = GameObject.Instantiate(tileGameObjects[tileName]) as GameObject;
        else
            newTileObj = new GameObject("Empty Tile");
        return newTileObj;
    }

    public void ReplaceTileType(string tileTypeName, TileType tileType)
    {
        var currentType = GetTileTypeByName(tileTypeName);
        this.types[this.types.FindIndex(k=>k.Equals(currentType))] =  tileType;
    }

}


public class TileTypeVolume
{
    public TileTypeVolumeType type;
    public double x;
    public double y;
    public double z;
    public int xScale;
    public int yScale;
    public int zScale;
    public bool isWall;

    public TileTypeVolume()
    {
        xScale = 100;
        yScale = 100;
        zScale = 100;
    }

    public TileTypeVolume(TileTypeVolumeType type)
    {
        this.type = type;
        xScale = 100;
        yScale = 100;
        zScale = 100;
    }
}


public class TileType
{
    public string name;
    public List<TileTypeVolume> volumes;
}
