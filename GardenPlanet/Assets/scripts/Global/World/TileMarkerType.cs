using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace GardenPlanet
{
    public class TileMarkerType
    {

        public string name;
        public Attributes defaultAttributes;
        [JsonIgnore]
        public Sprite sprite;

        /*
         * Returns a List containing all available TileMarker
         * objects.
         */
        public static List<TileMarkerType> GetAllTileMarkerTypes()
        {
            var filePath = Path.Combine(
                Consts.DATA_DIR,
                Consts.FILE_TILE_MARKERS_FILE
            );
            var tileMarkerTypes = new List<TileMarkerType>();
            if(File.Exists(filePath))
                using(var fh = File.OpenText(filePath))
                    tileMarkerTypes = JsonHandler.Deserialize<List<TileMarkerType>>(fh.ReadToEnd());
            foreach(var marker in tileMarkerTypes)
                marker.sprite = Resources.Load<Sprite>(Path.Combine("mapeditor/tilemarkers", marker.name));
            return tileMarkerTypes;
        }

        public static List<TileMarkerType> CreateEmptyTileMarkerDataFile()
        {
            var filePath = Path.Combine(
                Consts.DATA_DIR,
                Consts.FILE_TILE_MARKERS_FILE
            );
            if(!Directory.Exists(Consts.DATA_DIR))
                Directory.CreateDirectory(Consts.DATA_DIR);
            var exampleMarker = new List<TileMarkerType>();
            exampleMarker.Add(new TileMarkerType
                {
                    name= "Noot",
                    defaultAttributes=new Attributes{{"foo", "bar"}}
                }
            );
            JsonHandler.SerializeToFile(exampleMarker, filePath);
            return exampleMarker;
        }
    }
}