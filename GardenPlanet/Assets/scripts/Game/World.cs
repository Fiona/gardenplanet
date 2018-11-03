using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace GardenPlanet
{
    public class World : MonoBehaviour
    {
        public Map currentMap;
        public List<Map> maps;
        public TileTypeSet tileTypeSet;
        public WorldTimer timer;
        public Atmosphere atmosphere;
        public Tilemap tilemap;
        public MarkerManager markers;
        public WorldObjectManager objects;
        public TileTagManager tileTags;
        public Player player;
        public GameObject items;
        public GameObject charactersParent;
        public Dictionary<string, Character> characters;

        private Dictionary<string, WorldObject> bedObjects;

        private void Awake()
        {
            bedObjects = new Dictionary<string, WorldObject>();

            // Init characters
            characters = new Dictionary<string, Character>();
            charactersParent = new GameObject("Characters");
            charactersParent.transform.SetParent(transform, true);

            // Set up maps and default map
            tileTypeSet = new TileTypeSet("default");
            maps = new List<Map>();
            currentMap = new Map(App.Instance.appSettings.StartMap);
            maps.Add(currentMap);

            // World timer
            var mainCanvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
            var worldTimerObject = Instantiate(Resources.Load(Consts.PREFAB_PATH_WORLD_TIMER)) as GameObject;
            worldTimerObject.transform.SetParent(mainCanvas.transform, false);
            worldTimerObject.transform.SetSiblingIndex(worldTimerObject.transform.GetSiblingIndex() - 1);
            timer = worldTimerObject.GetComponent<WorldTimer>();

            // Atmosphere
            var atmosphereObj = Instantiate(Resources.Load(Consts.PREFAB_PATH_ATMOSPHERE)) as GameObject;
            atmosphereObj.transform.SetParent(transform, true);
            atmosphere = atmosphereObj.GetComponent<Atmosphere>();

            // Tilemap
            var tilemapObj = new GameObject("Tilemap");
            tilemapObj.transform.SetParent(transform, true);
            tilemap = tilemapObj.AddComponent<Tilemap>();
            tilemap.LoadFromMap(currentMap, tileTypeSet);

            // Markers
            var markerManagerObj = new GameObject("MarkerManager");
            markerManagerObj.transform.SetParent(transform, true);
            markers = markerManagerObj.AddComponent<MarkerManager>();
            markers.LoadFromMap(currentMap);

            // World objects
            var worldObjectManagerObj = new GameObject("ObjectManager");
            worldObjectManagerObj.transform.SetParent(transform, true);
            objects = worldObjectManagerObj.AddComponent<WorldObjectManager>();
            objects.LoadFromMap(currentMap);

            // Tile Tags
            var tileTagManagerObj = new GameObject("TileTagManager");
            tileTagManagerObj.transform.SetParent(transform, true);
            tileTags = tileTagManagerObj.AddComponent<TileTagManager>();
            tileTags.LoadFromMap(currentMap);

            // In world items
            items = new GameObject("Items");
            items.transform.SetParent(transform, true);
        }

        private void Start()
        {
            timer.StartTimer();
        }

        /*
         * Updates a characters bed data, making sure the relevant objects are created
         */
        public void SetBed(string characterId, Character.Information.Bed bedInformation)
        {
            // TODO: replace marker in map data and support multiple maps
            if(bedObjects.ContainsKey(characterId))
                objects.DeleteWorldObject(bedObjects[characterId]);
            bedObjects[characterId] = objects.AddWorldObject(
                objects.GetWorldObjectTypeByName(bedInformation.type),
                bedInformation.location,
                new Attributes {{"owner", characterId}}
            );
        }

        /*
         * Generates an item in the world and returns it
         */
        public InWorldItem SpawnItem(ItemType itemType, Attributes attributes)
        {
            var resource = Resources.Load<GameObject>(Consts.ITEMS_PREFABS_PATH + itemType.Appearance);
            resource = resource ?? Resources.Load<GameObject>(Consts.ITEMS_PREFAB_MISSING);
            var newItem = Instantiate(resource);
            newItem.transform.parent = items.transform;
            var newItemComponent = newItem.AddComponent<InWorldItem>();
            newItemComponent.attributes = attributes;
            newItemComponent.itemType = itemType;
            return newItemComponent;
        }

        /*
         * Gives back bed information for the passed owner name.
         * Null if owner requested does not have a bed set in the world.
         */
        public Character.Information.Bed GetBedOwnedBy(string owner)
        {
            foreach(var map in maps)
            {
                foreach(var marker in map.markers)
                {
                    if(marker.type != "Bed" || !marker.attributes.Contains("owner") ||
                       marker.attributes.Get<string>("owner") != owner)
                        continue;
                    var tilePos = new TilePosition(marker.x, marker.y, marker.layer) {dir = marker.direction};
                    return new Character.Information.Bed
                    {
                        location =  new MapWorldPosition(map, tilePos),
                        type = marker.attributes.Get<string>("type")
                    };
                }
            }
            return null;
        }

        /*
         * Tries to create the specified character. If the character cannot be created, null is returned.
         * Will not create duplicate characters with the same ID.
         */
        public Character AddCharacter(string ID, MapTilePosition spawnLocation = null, EightDirection? direction = null)
        {
            if(ID == "" || characters.ContainsKey(ID))
                return null;

            // Create and add the character game object
            var prefabPath = ID == Consts.CHAR_ID_PLAYER ? Consts.PREFAB_PATH_PLAYER : Consts.PREFAB_PATH_CHARACTER;
            var character = Instantiate(Resources.Load<Character>(prefabPath));
            character.transform.SetParent(charactersParent.transform);
            character.id = ID;
            characters.Add(ID, character);

            // Load data in
            if(ID != Consts.CHAR_ID_PLAYER)
            {
                var dataFilepath = character.GetCharacterDataFilePath();
                if(File.Exists(dataFilepath))
                {
                    Character.CharacterData loadedData;
                    try
                    {
                        using(var fh = File.OpenText(dataFilepath))
                            loadedData = JsonHandler.Deserialize<Character.CharacterData>(fh.ReadToEnd());
                        character.SetCharacterData(loadedData);
                    }
                    catch(JsonErrorException e)
                    {
                        Debug.Log(e);
                        throw new Exception($"Error loading character data: {dataFilepath}");
                    }
                }
                else
                    throw new Exception($"Character data file does not exist: {dataFilepath}");
            }
            else
            {
                player = character as Player;
            }

            // Set position
            if(spawnLocation == null)
                character.currentMap = currentMap;
            else
                character.SetPositionToTile(spawnLocation, spawnLocation.map);

            // Set direction
            if(direction != null)
                character.SetRotation(DirectionHelper.DirectionToDegrees(direction.Value));

            // Create bed if applicable
            var bedInfo = character.GetBedInformation() ?? GetBedOwnedBy(ID);
            if(bedInfo != null)
                character.SetBed(bedInfo);

            // Add AI component
            character.gameObject.AddComponent<CharacterAI>();

            return character;
        }
    }
}