using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using StompyBlondie;
using StompyBlondie.Common.Types;
using Debug = UnityEngine.Debug;
using StompyBlondie.AI;

namespace GardenPlanet
{

    public class Map
    {

        public struct MapTile
        {
            public int x;
            public int y;
            public int layer;
            public string type;
            public EightDirection direction;
        }

        public struct Marker
        {
            public int x;
            public int y;
            public int layer;
            public string type;
            public EightDirection direction;
            public Attributes attributes;
        }

        public struct WorldObject
        {
            public double x;
            public double y;
            public double height;
            public string type;
            public EightDirection direction;
            public Attributes attributes;
        }

        public int width;
        public int height;
        public string filename;
        public string fullFilepath;
        public IList<MapTile> tiles;
        public IList<Marker> markers;
        public IList<WorldObject> worldObjects;
        public IList<TileTag> mapTags;
        public NavigationMap navigationMap = new NavigationMap();

        public Map()
        {
            Init();
        }

        public void Init()
        {
            width = 20;
            height = 20;
            tiles = new List<MapTile>();
            markers = new List<Marker>();
            worldObjects = new List<WorldObject>();
            mapTags = new List<TileTag>();
            navigationMap = new NavigationMap();
        }

        public Map(string filename)
        {
            this.filename = filename;
            Init();

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
                        loadedMap = JsonHandler.Deserialize<Map>(fh.ReadToEnd());
                }
                catch(JsonErrorException e)
                {
                    Debug.Log(e);
                    throw new EditorErrorException("Error loading map.");
                }
                tiles = loadedMap.tiles;
                markers = loadedMap.markers;
                worldObjects = loadedMap.worldObjects;
                mapTags = loadedMap.mapTags;
                width = loadedMap.width;
                height = loadedMap.height;
                navigationMap = loadedMap.navigationMap;
            }
            else
                throw new IOException($"Map file does not exist: {fullFilepath}");
        }

        // Constructor for converting from tilemap
        public Map(string filename, Tilemap tilemap)
        {
            this.filename = filename;
            fullFilepath = GetMapFilePathFromName(filename);
            width = tilemap.width;
            height = tilemap.height;
            Init();

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
                jsonOutput = JsonHandler.Serialize(this);
            }
            catch(JsonErrorException e)
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

        public static Map NewMap(string filename, int width = 20, int height = 20)
        {
            var map = new Map();
            map.filename = filename;
            map.width = width;
            map.height = height;
            map.fullFilepath = GetMapFilePathFromName(filename);
            return map;
        }

    }

}