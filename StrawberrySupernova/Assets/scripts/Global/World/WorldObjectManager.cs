using System;
using UnityEngine;
using System.Collections.Generic;

namespace StrawberryNova
{
	public class WorldObjectManager: MonoBehaviour
	{

		public List<WorldObjectType> worldObjectTypes;
		public List<ObjectWorldPosition> worldObjects;

		static PhysicMaterial slideMaterial;

		public void Awake()
		{
			if(WorldObjectManager.slideMaterial == null)
				WorldObjectManager.slideMaterial = (PhysicMaterial)Resources.Load("SlideMaterial") as PhysicMaterial;			
			worldObjectTypes = WorldObjectType.GetAllWorldObjectTypes();
			worldObjects = new List<ObjectWorldPosition>();
		}
			
		/*
	      Initialises using the passed Map object.
	    */
		public void LoadFromMap(Map map)
		{
			// Destroy old ones
			if(worldObjects.Count > 0)
			{
				var worldObjectsClone = new List<ObjectWorldPosition>(worldObjects);
				foreach(var obj in worldObjectsClone)
					DeleteWorldObject(obj);
				worldObjects = new List<ObjectWorldPosition>();
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

		public void AddWorldObject(WorldObjectType objectType, WorldPosition pos)
		{
			if(objectType == null)
				return;

			// Create game object
			GameObject newGameObject = null;

			newGameObject = Instantiate(objectType.prefab);
			newGameObject.transform.parent = transform;
			newGameObject.transform.localPosition = new Vector3(pos.x, pos.height, pos.y);
			newGameObject.layer = Consts.COLLISION_LAYER_WORLD_OBJECTS;

			var newWorldObject = new ObjectWorldPosition {
				x = pos.x,
				y = pos.y,
				height = pos.height,
				gameObject = newGameObject,
				name = objectType.name
			};

			worldObjects.Add(newWorldObject);
			SetWorldObjectDirection(newWorldObject, pos.dir);

			if(FindObjectOfType<App>().state == AppState.Game && objectType.interactable)
			{
				var interactable = newGameObject.AddComponent<WorldObjectInteractable>();
				interactable.worldObject = newWorldObject;
			}

			var comp = newGameObject.GetComponentInChildren<BoxCollider>();
			if(comp != null)
				comp.material = WorldObjectManager.slideMaterial;

		}

		public void DeleteWorldObject(ObjectWorldPosition worldObjectToDelete)
		{
			if(worldObjects.Exists(x => x == worldObjectToDelete))
			{
				Destroy(worldObjectToDelete.gameObject);
				worldObjects.Remove(worldObjectToDelete);
			}
		}

		public void AddWorldObject(WorldObjectType objectType, TilePosition pos)
		{
			var tilemap = FindObjectOfType<Tilemap>();
			if(pos.x < 0 || pos.x >= tilemap.width || pos.y < 0 || pos.y >= tilemap.height)
				throw new EditorErrorException("Object outside of tilemap.");
			AddWorldObject(objectType, new WorldPosition{ x=pos.x, y=pos.y, height=(pos.layer * Consts.TILE_HEIGHT)});
		}

		public void SetWorldObjectDirection(ObjectWorldPosition worldObject, EightDirection direction)
		{
			worldObject.dir = direction;
			if(worldObject.gameObject != null)
			{
				var baseRotation = DirectionHelper.DirectionToDegrees(direction);
				worldObject.gameObject.transform.localRotation = Quaternion.Euler(0, baseRotation, 0);
			}
		}

		public void TurnWorldObjectInDirection(ObjectWorldPosition worldObjectMoving, RotationalDirection dir)
		{
			SetWorldObjectDirection(worldObjectMoving, DirectionHelper.RotateDirection(worldObjectMoving.dir, dir));
		}

		public ObjectWorldPosition GetWorldObjectByGameObject(GameObject obj)
		{
			if(obj == null)
				return null;
			foreach(var worldObject in worldObjects)
				if(worldObject.gameObject == obj)
					return worldObject;
			return null;
		}

	}
}