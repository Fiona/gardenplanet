using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;
using StompyBlondie;

namespace StrawberryNova
{
    public class WorldObjectManager: MonoBehaviour
    {

        static PhysicMaterial slideMaterial;

        public List<WorldObjectType> worldObjectTypes;
        public List<WorldObject> worldObjects;

        private Dictionary<string, List<WorldObject>> tilePosToWorldObjects;
        private List<WorldObject> lateDeleteWorldObjects;

        public void Awake()
        {
            if(WorldObjectManager.slideMaterial == null)
                WorldObjectManager.slideMaterial = (PhysicMaterial)Resources.Load("SlideMaterial") as PhysicMaterial;
            worldObjectTypes = WorldObjectType.GetAllWorldObjectTypes();
            worldObjects = new List<WorldObject>();
            lateDeleteWorldObjects = new List<WorldObject>();
            tilePosToWorldObjects = new Dictionary<string, List<WorldObject>>();
        }

        public void LateUpdate()
        {
            foreach(var obj in new List<WorldObject>(lateDeleteWorldObjects))
                DeleteWorldObject(obj);
            lateDeleteWorldObjects = new List<WorldObject>();
        }

        /*
          Initialises using the passed Map object.
        */
        public void LoadFromMap(Map map)
        {
            // Destroy old ones
            if(worldObjects.Count > 0)
            {
                var worldObjectsClone = new List<WorldObject>(worldObjects);
                foreach(var obj in worldObjectsClone)
                    DeleteWorldObject(obj);
                worldObjects = new List<WorldObject>();
            }

            // Load and create all world objects
            foreach(var worldObject in map.worldObjects)
            {
                var pos = new WorldPosition {
                    x=(float)worldObject.x,
                    y=(float)worldObject.y,
                    height=(float)worldObject.height,
                    dir=worldObject.direction
                };
                AddWorldObject(GetWorldObjectTypeByName(worldObject.type), pos);
            }
        }

        public void SaveToMap(Map map)
        {
            foreach(var worldObject in worldObjects)
            {
                var newWorldObject = new Map.WorldObject(){
                    x=(double)worldObject.x,
                    y=(double)worldObject.y,
                    height=(double)worldObject.height,
                    direction=worldObject.dir,
                    type=worldObject.name
                };
                map.worldObjects.Add(newWorldObject);
            }
        }

        public WorldObjectType GetWorldObjectTypeByName(string name)
        {
            if(name == null)
                return null;
            foreach(var type in this.worldObjectTypes)
                if(type.name == name)
                    return type;
            throw new EditorErrorException("Couldn't find world object type requested: " + name);
        }

        public WorldObjectType GetNextWorldObjectMarkerType(WorldObjectType currentWorldObjectType)
        {
            if(worldObjectTypes.Count == 1)
                return currentWorldObjectType;
            var index = (worldObjectTypes.IndexOf(currentWorldObjectType) + 1)
                        % Math.Max(worldObjectTypes.Count, 2);
            return worldObjectTypes[index];
        }

        public WorldObjectType GetPreviousWorldObjectMarkerType(WorldObjectType currentWorldObjectType)
        {
            if(worldObjectTypes.Count == 1)
                return currentWorldObjectType;
            var index = (worldObjectTypes.IndexOf(currentWorldObjectType) - 1)
                        % Math.Max(worldObjectTypes.Count, 2);
            return worldObjectTypes[index];
        }

        public WorldObject AddWorldObject(WorldObjectType objectType, WorldPosition pos, Hashtable attributes = null)
        {
            if(objectType == null)
                return null;

            // Create game object
            GameObject newGameObject = null;

            newGameObject = new GameObject(objectType.name);
            newGameObject.transform.parent = transform;
            newGameObject.transform.localPosition = new Vector3(pos.x, pos.height, pos.y);
            newGameObject.layer = Consts.COLLISION_LAYER_WORLD_OBJECTS;

            var newWorldObject = new WorldObject {
                x = pos.x,
                y = pos.y,
                height = pos.height,
                gameObject = newGameObject,
                name = objectType.name,
                objectType = objectType,
                attributes = new Hashtable(objectType.defaultAttributes)
            };
            if(attributes != null)
                foreach(DictionaryEntry attr in attributes)
                    newWorldObject.attributes[attr.Key] = attr.Value;

            worldObjects.Add(newWorldObject);
            SetWorldObjectDirection(newWorldObject, pos.dir);

            // If we're in the game then we need to set up behaviours and scripts
            if(FindObjectOfType<App>().state == AppState.Game)
            {
                // Behaviour for interactables
                if(objectType.interactable)
                {
                    var interactable = newGameObject.AddComponent<WorldObjectInteractable>();
                    interactable.worldObject = newWorldObject;
                }
                // Script
                if(!String.IsNullOrEmpty(objectType.script))
                {
                    var script = Type.GetType(typeof(WorldObjectManager).Namespace+"."+objectType.script);
                    if(script != null)
                    {
                        var newComponent = newWorldObject.gameObject.AddComponent(script);
                        newWorldObject.script = newComponent as WorldObjectScript;
                        newWorldObject.script.worldObject = newWorldObject;
                    }
                }
            }

            // Set up physics material
            var comp = newGameObject.GetComponentInChildren<BoxCollider>();
            if(comp != null)
                comp.material = WorldObjectManager.slideMaterial;

            // Set up appearence
            newWorldObject.SetAppearence();

            return newWorldObject;

        }

        public WorldObject AddWorldObject(WorldObjectType objectType, TilePosition pos, Hashtable attributes = null)
        {
            var tilemap = FindObjectOfType<Tilemap>();
            if(pos.x < 0 || pos.x >= tilemap.width || pos.y < 0 || pos.y >= tilemap.height)
                throw new EditorErrorException("Object outside of tilemap.");
            var worldPos = new WorldPosition {
                x=pos.x * Consts.TILE_SIZE,
                y=pos.y * Consts.TILE_SIZE,
                height=pos.layer * Consts.TILE_SIZE
            };
            var newWorldObject = AddWorldObject(objectType, worldPos, attributes);
            if(newWorldObject == null)
                return null;

            // Sort out tile pos tracking
            if(objectType.tileObject)
            {
                var posS = pos.ToString();
                if(!tilePosToWorldObjects.ContainsKey(posS))
                    tilePosToWorldObjects[posS] = new List<WorldObject>();
                tilePosToWorldObjects[posS].Add(newWorldObject);
            }
            return newWorldObject;
        }

        public void DeleteWorldObject(WorldObject worldObjectToDelete)
        {
            if(worldObjectToDelete.objectType.tileObject)
            {
                foreach(var list in tilePosToWorldObjects)
                {
                    foreach(var obj in new List<WorldObject>(list.Value))
                    {
                        if(obj == worldObjectToDelete)
                        {
                            tilePosToWorldObjects[list.Key].Remove(worldObjectToDelete);
                        }
                    }
                }

                foreach(var list in new Dictionary<string, List<WorldObject>>(tilePosToWorldObjects))
                    if(list.Value.Count == 0)
                        tilePosToWorldObjects.Remove(list.Key);
            }

            if(worldObjects.Exists(x => x == worldObjectToDelete))
            {
                Destroy(worldObjectToDelete.gameObject);
                worldObjects.Remove(worldObjectToDelete);
            }
        }


        public void LateDeleteWorldObject(WorldObject worldObjectToDelete)
        {
            lateDeleteWorldObjects.Add(worldObjectToDelete);
        }

        public void SetWorldObjectDirection(WorldObject worldObject, EightDirection direction)
        {
            worldObject.dir = direction;
            if(worldObject.gameObject != null)
            {
                var baseRotation = DirectionHelper.DirectionToDegrees(direction);
                worldObject.gameObject.transform.localRotation = Quaternion.Euler(0, baseRotation, 0);
            }
        }

        public void TurnWorldObjectInDirection(WorldObject worldObjectMoving, RotationalDirection dir)
        {
            SetWorldObjectDirection(worldObjectMoving, DirectionHelper.RotateDirection(worldObjectMoving.dir, dir));
        }

        public WorldObject GetWorldObjectByGameObject(GameObject obj)
        {
            if(obj == null)
                return null;
            foreach(var worldObject in worldObjects)
                if(worldObject.gameObject == obj)
                    return worldObject;
            return null;
        }

        public List<WorldObject> GetWorldObjectsAtTilePos(TilePosition tilePos)
        {
            var posS = tilePos.ToString();
            if(tilePosToWorldObjects.ContainsKey(posS))
                return new List<WorldObject>(tilePosToWorldObjects[posS]);
            return new List<WorldObject>();
        }

    }
}