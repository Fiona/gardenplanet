using System;
using UnityEngine;
using System.Collections.Generic;

namespace StrawberryNova
{
	public class MarkerManager: MonoBehaviour
	{

		public List<TileMarkerType> tileMarkerTypes;
		public List<ObjectTilePosition> tileMarkers;

		public void Awake()
		{
			tileMarkerTypes = TileMarkerType.GetAllTileMarkerTypes();
			tileMarkers = new List<ObjectTilePosition>();
		}

		/*
	      Initialises the tilemap using the passed Map object.
	    */
		public void LoadFromMap(Map map)
		{
			// Destroy old one
			if(tileMarkers.Count > 0)
			{
				var markerListClone = new List<ObjectTilePosition>(tileMarkers);
				foreach(var marker in markerListClone)
					RemoveMarkerAt(marker.x, marker.y, marker.layer);
				tileMarkers = new List<ObjectTilePosition>();
			}

			// Load in markers
			foreach(var marker in map.markers)
				AddMarkerAt(
					GetTileMarkerTypeByName(marker.type),
					marker.x, marker.y, marker.layer, marker.direction
				);
		}

		public void SaveToMap(Map map)
		{
			foreach(var marker in tileMarkers)
			{
				var newMarker = new Map.Marker(){
					x=marker.x,
					y=marker.y,
					layer=marker.layer,
					direction=marker.dir,
					type=marker.name
				};
				map.markers.Add(newMarker);
			}			
		}

		public TileMarkerType GetTileMarkerTypeByName(string name)
		{
			if(name == null)
				return null;
			foreach(var type in this.tileMarkerTypes)
				if(type.name == name)
					return type;
			throw new EditorErrorException("Couldn't find marker type requested.");
		}

		public TileMarkerType GetNextTileMarkerType(TileMarkerType currentTileMarker)
		{
			var index = (tileMarkerTypes.IndexOf(currentTileMarker) + 1)
				% Math.Max(tileMarkerTypes.Count, 2);
			return tileMarkerTypes[index];
		}

		public TileMarkerType GetPreviousTileMarkerType(TileMarkerType currentTileMarker)
		{
			var index = Math.Abs((tileMarkerTypes.IndexOf(currentTileMarker) - 1)
				% Math.Max(tileMarkerTypes.Count, 2));
			return tileMarkerTypes[index];
		}

		public void RemoveMarkerAt(int x, int y, int layer)
		{
			var marker = GetMarkerAt(x, y, layer);
			if(marker == null)
				return;
			Destroy(marker.gameObject);
			tileMarkers.Remove(marker);
		}

		public void AddMarkerAt(TileMarkerType markerType, int x, int y, int layer, Direction dir)
		{

			if(markerType == null)
				return;
			
			var tilemap = FindObjectOfType<Tilemap>();
			if(x < 0 || x >= tilemap.width || y < 0 || y >= tilemap.height)
				throw new EditorErrorException("Marker outside of tilemap.");

			// Create game object
			GameObject newGameObject = null;
			if(FindObjectOfType<App>().state == AppState.Editor)
			{
				var markerTemplate = Resources.Load<GameObject>("Editor/TileMarker");
				newGameObject = Instantiate(markerTemplate);
				newGameObject.transform.parent = transform;
				newGameObject.transform.localPosition = new Vector3(
                    x * Consts.TILE_SIZE,
					layer * Consts.TILE_SIZE,
                    y * Consts.TILE_SIZE);
				newGameObject.GetComponentInChildren<MeshRenderer>().material.SetTexture(
					"_MainTex",
					markerType.sprite.texture
				);
			}

			// Create new marker display
			var newMarker = new ObjectTilePosition{
				x = x,
				y = y,
				layer = layer,
				gameObject = newGameObject,
				name = markerType.name
			};
			tileMarkers.Add(newMarker);
			SetMarkerDirection(newMarker, dir);

		}

		public ObjectTilePosition GetMarkerAt(int x, int y, int layer)
		{
			foreach(var marker in tileMarkers)
				if(marker.x == x && marker.y == y && marker.layer == layer)
					return marker;
			return null;
		}

		public void ResizeMap(int width, int height)
		{
			var markersToKill = new List<ObjectTilePosition>();
			foreach(var marker in tileMarkers)
				if(marker.x >= width || marker.y >= height)
					markersToKill.Add(marker);
			foreach(var marker in markersToKill)
				RemoveMarkerAt(marker.x, marker.y, marker.layer);			
		}

		public void RotateMarkerInDirection(ObjectTilePosition marker, RotationalDirection rot)
		{
			SetMarkerDirection(marker, DirectionHelper.RotateDirection(marker.dir, rot));
		}

		public void SetMarkerDirection(ObjectTilePosition marker, Direction direction)
		{
			marker.dir = direction;
			if(marker.gameObject != null)
			{
				var baseRotation = DirectionHelper.DirectionToDegrees(direction);
				marker.gameObject.transform.localRotation = Quaternion.Euler(0, baseRotation, 0);
			}
		}

		public ObjectTilePosition GetFirstTileMarkerOfType(string markerType)
		{
			foreach(var marker in tileMarkers)
				if(marker.name == markerType)
					return marker;
			return null;
		}

	}
}

