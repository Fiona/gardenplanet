using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StompyBlondie;
using UnityEngine;
using StompyBlondie.AI;
using StompyBlondie.Common.Types;
using StompyBlondie.Extensions;

namespace GardenPlanet
{

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
                        loadedSet = JsonHandler.Deserialize<TileTypeSet>(fh.ReadToEnd());
                }
                catch(JsonErrorException e)
                {
                    Debug.Log(e);
                    throw new EditorErrorException("Error loading set.");
                }
                this.name = loadedSet.name;
                this.types = loadedSet.types;
            }

            var resaveTileSet = false;

            // Update with new tiles
            var allTileTypes = Resources.LoadAll(Consts.TILES_PREFABS_PATH);
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
                jsonOutput = JsonHandler.Serialize(this);
            }
            catch(JsonErrorException e)
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
        public TileTypeVolumeShape shape;
        public double x;
        public double y;
        public double z;
        public int xScale;
        public int yScale;
        public int zScale;
        public TileTypeVolumeSurface surface;

        public TileTypeVolume()
        {
            xScale = 100;
            yScale = 100;
            zScale = 100;
        }

        public TileTypeVolume(TileTypeVolumeShape shape)
        {
            this.shape = shape;
            xScale = 100;
            yScale = 100;
            zScale = 100;
        }
    }


    public class TileType
    {
        public string name;
        public List<TileTypeVolume> volumes;
        public double xCentre;
        public double yCentre;
        public double zCentre;
        public int[] size = {1,1};
        public string autoTag;
        public NavigationMap navigationMap = new NavigationMap();

        public void CalculateNavPoints()
        {
            navigationMap.Reset();
            var halfTileSize = Consts.TILE_SIZE / 2;

            // Add all relevant points
            var points = new List<Pos>();
            for(var tileX = 0; tileX < size[0]; tileX++)
            {
                for(var tileY = 0; tileY < size[1]; tileY++)
                {
                    for(var x = -halfTileSize; x <= halfTileSize; x += halfTileSize)
                    {
                        for(var y = -halfTileSize; y <= halfTileSize; y += halfTileSize)
                        {
                            points.Add(new Pos(x + (tileX * Consts.TILE_SIZE), y + (tileY * Consts.TILE_SIZE), 0f));
                            navigationMap.AddPoint(points.Last());
                        }
                    }
                }
            }

            // Add links between all points
            foreach(var p in points)
            {
                foreach(var p2 in points)
                {
                    // Skip same point
                    if(p == p2)
                        continue;
                    // Skip if point is too far away
                    var distanceCutOff = halfTileSize * Mathf.Sqrt(2);
                    var pointDistance = navigationMap.DistanceBetweenPoints(p, p2);
                    if(pointDistance > distanceCutOff)
                        continue;
                    // Slightly different cost based on longer point distances
                    var cost = Mathf.Max(1f, pointDistance / halfTileSize);
                    navigationMap.AddPointLink(p, p2, cost);
                }
            }

            // Remove points by seeing if they overlap with wall volumes
            foreach(var vol in volumes)
            {
                // Only walls and boxes are supported
                if(vol.surface != TileTypeVolumeSurface.WALL || vol.shape != TileTypeVolumeShape.CollisionBox)
                    continue;
                // Create bounding box
                var volumeSize = new Vector3(
                    (Consts.VOLUME_SCALE_DEFAULT / 100.0f) * vol.xScale + Consts.CHARACTER_RADIUS,
                    (Consts.VOLUME_SCALE_DEFAULT / 100.0f) * vol.yScale + Consts.CHARACTER_RADIUS,
                    (Consts.VOLUME_SCALE_DEFAULT / 100.0f) * vol.zScale + Consts.CHARACTER_RADIUS
                );
                var volumeCentre = new Vector3(
                    Consts.VOLUME_POSITION_SHIFT_PER_UNIT * (float)vol.x,
                    Consts.VOLUME_POSITION_SHIFT_PER_UNIT * (float)vol.y,
                    Consts.VOLUME_POSITION_SHIFT_PER_UNIT * (float)vol.z
                    );
                var volumeBounds = new Bounds(volumeCentre, volumeSize);
                // Find out if any of the points are inside it
                var pointsToRemove = new List<Pos>();
                foreach(var p in points)
                {
                    var positionOnTile = p.ToVector3() - new Vector3((float)xCentre, (float)yCentre, (float)zCentre);
                    if(volumeBounds.Contains(positionOnTile))
                        pointsToRemove.Add(p);
                }
                // Remove any points found
                foreach(var p in pointsToRemove)
                    navigationMap.RemovePoint(p);
            }
        }
    }

}