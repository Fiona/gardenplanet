using UnityEngine;
using System.Collections.Generic;

namespace StrawberryNova
{
    public class TileTagType
    {
        
        public string ID;
        public Sprite sprite;
        
        /*
         * Returns a List containing all available TileTag objects.
         */
        public static List<TileTagType> GetAllTileTagTypes()
        {
            var tileTags = new List<TileTagType>();
            var allTagTypes = Resources.LoadAll<Sprite>("mapeditor/tiletags/");
            foreach(var filesystemType in allTagTypes)
            {				
                var newTagType = new TileTagType
                {
                    ID=filesystemType.name,
                    sprite=filesystemType
                };
                tileTags.Add(newTagType);
            }
            return tileTags;
        }
        
    }
}