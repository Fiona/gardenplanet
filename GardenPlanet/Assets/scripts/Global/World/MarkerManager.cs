using System;
using UnityEngine;
using System.Collections.Generic;
using StompyBlondie.Common.Types;
using StompyBlondie.Utils;

namespace GardenPlanet
{
	public class MarkerManager: MonoBehaviour
	{

		public List<TileMarkerType> tileMarkerTypes;
		public List<TileMarker> tileMarkers;

		public void Awake()
		{
			tileMarkerTypes = TileMarkerType.GetAllTileMarkerTypes();
			tileMarkers = new List<TileMarker>();
		}

		/*
	      Initialises the tilemap using the passed Map object.
	    */
		public void LoadFromMap(Map map)
		{
			// Destroy old one
			if(tileMarkers.Count > 0)
			{
				var markerListClone = new List<TileMarker>(tileMarkers);
				foreach(var marker in markerListClone)
					RemoveMarkerAt(marker.x, marker.y, marker.layer);
				tileMarkers = new List<TileMarker>();
			}

			// Load in markers
			foreach(var marker in map.markers)
				AddMarkerAt(
					GetTileMarkerTypeByName(marker.type),
					marker.x, marker.y, marker.layer, marker.direction, marker.attributes
				);
		}

		public void SaveToMap(Map map)
		{
			foreach(var marker in tileMarkers)
			{
				var newMarker = new Map.Marker{
					x=marker.x,
					y=marker.y,
					layer=marker.layer,
					direction=marker.direction,
					type=marker.type.name,
					attributes=new Attributes(marker.attributes)
				};
				map.markers.Add(newMarker);
			}
		}

		public TileMarkerType GetTileMarkerTypeByName(string name)
		{
			if(name == null)
				return null;
			foreach(var type in tileMarkerTypes)
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

		public void AddMarkerAt(TileMarkerType markerType, int x, int y, int layer, EightDirection dir, Attributes attributes = null)
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
				var markerTemplate = Resources.Load<GameObject>("mapeditor/TileMarker");
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
			var attrs = new Attributes(attributes ?? markerType.defaultAttributes);
			var newMarker = new TileMarker{
				x = x,
				y = y,
				layer = layer,
				gameObject = newGameObject,
				attributes = attrs,
				type = markerType
			};
			tileMarkers.Add(newMarker);
			SetMarkerDirection(newMarker, dir);
		}

		public TileMarker GetMarkerAt(int x, int y, int layer)
		{
			foreach(var marker in tileMarkers)
				if(marker.x == x && marker.y == y && marker.layer == layer)
					return marker;
			return null;
		}

		public void ResizeMap(int width, int height)
		{
			var markersToKill = new List<TileMarker>();
			foreach(var marker in tileMarkers)
				if(marker.x >= width || marker.y >= height)
					markersToKill.Add(marker);
			foreach(var marker in markersToKill)
				RemoveMarkerAt(marker.x, marker.y, marker.layer);
		}

		public void RotateMarkerInDirection(TileMarker marker, RotationalDirection rot)
		{
			SetMarkerDirection(marker, DirectionHelper.RotateDirection(marker.direction, rot));
		}

		public void SetMarkerDirection(TileMarker marker, EightDirection direction)
		{
			marker.direction = direction;
			if(marker.gameObject != null)
			{
				var baseRotation = DirectionHelper.DirectionToDegrees(direction);
				marker.gameObject.transform.localRotation = Quaternion.Euler(0, baseRotation, 0);
			}
		}

		public TileMarker GetFirstTileMarkerOfType(TileMarkerType type)
		{
			foreach(var marker in tileMarkers)
				if(marker.type == type)
					return marker;
			return null;
		}

		public List<TileMarker> GetMarkersOfType(TileMarkerType type)
		{
			var markers = new List<TileMarker>();
			foreach(var marker in tileMarkers)
				if(marker.type == type)
					markers.Add(marker);
			return markers;
		}

	}
}

