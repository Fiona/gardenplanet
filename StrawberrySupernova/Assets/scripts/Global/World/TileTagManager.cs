using System;
using UnityEngine;
using System.Collections.Generic;

namespace StrawberryNova
{
    public class TileTagManager : MonoBehaviour
    {

        public List<TileTagType> TileTagTypes;
        public List<TileTag> TileTags;
        public Dictionary<TileTag, GameObject> EditorTileTagObjects;

        public void Awake()
        {
            TileTagTypes = TileTagType.GetAllTileTagTypes();
            TileTags = new List<TileTag>();
            EditorTileTagObjects = new Dictionary<TileTag, GameObject>();
        }

        /*
          Clears all existing tags and puts the ones passed from the map in.
        */
        public void LoadFromMap(Map map)
        {
            // Destroy old one
            if(TileTags.Count > 0)
            {
                var tagListClone = new List<TileTag>(TileTags);
                foreach(var tag in tagListClone)
                    RemoveTag(tag);
                TileTags = new List<TileTag>();
            }

            // Add tags from the map
            foreach(var tag in map.mapTags)
                AddTag(tag);
        }

        public void SaveToMap(Map map)
        {
            foreach(var tag in TileTags)
            {
                var newTag = tag;
                map.mapTags.Add(newTag);
            }
        }

        public void AddTag(TileTag tag)
        {
            var newTag = tag;
            TileTags.Add(newTag);

            // Display tag in editor
            if(FindObjectOfType<App>().state == AppState.Editor)
            {
                GameObject newGameObject = null;
                var tagTemplate = Resources.Load<GameObject>("mapeditor/TileTag");
                newGameObject = Instantiate(tagTemplate);
                newGameObject.transform.parent = transform;
                newGameObject.transform.localPosition = new Vector3(
                    tag.X * Consts.TILE_SIZE,
                    (tag.Layer * Consts.TILE_SIZE) + .005f,
                    tag.Y * Consts.TILE_SIZE);
                var tagType = TileTagTypes.Find((n) => n.ID == tag.TagType);
                newGameObject.GetComponent<MeshRenderer>().material.SetTexture(
                    "_MainTex",
                    tagType.sprite.texture
                );
                EditorTileTagObjects[newTag] = newGameObject;
            }
        }

        public void RemoveTag(TileTag tag)
        {
            if(TileTags.Contains(tag))
                TileTags.Remove(tag);
            if(EditorTileTagObjects.ContainsKey(tag))
            {
                Destroy(EditorTileTagObjects[tag]);
                EditorTileTagObjects.Remove(tag);
            }
        }

        public TileTag? FindTagAt(int x, int y, int layer)
        {
            foreach(var tag in TileTags)
            {
                if(tag.X == x && tag.Y == y && tag.Layer == layer)
                    return tag;
            }
            return null;
        }

        public bool IsTileTaggedWith(TilePosition tilePos, string tagName)
        {
            var tag = FindTagAt(tilePos.x, tilePos.y, tilePos.layer);
            return tag != null && tag.Value.TagType == tagName;
        }

        public void ResizeMap(int width, int height)
        {
            var tagsToKill = new List<TileTag>();
            foreach(var tag in TileTags)
                if(tag.X >= width || tag.Y >= height)
                    tagsToKill.Add(tag);
            foreach(var tag in tagsToKill)
                RemoveTag(tag);
        }

    }
}