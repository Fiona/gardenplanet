using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

namespace GardenPlanet
{
	public class TileMarkerType
	{

		public string name;
		public Sprite sprite;

		/*
		 * Returns a List containing all available TileMarker
		 * objects.
		 */
		public static List<TileMarkerType> GetAllTileMarkerTypes()
		{
			var tileMarkers = new List<TileMarkerType>();
			var allMarkerTypes = Resources.LoadAll<Sprite>("mapeditor/tilemarkers/");
			foreach(var filesystemType in allMarkerTypes)
			{				
				var newMarkerType = new TileMarkerType
					{
						name=filesystemType.name,
						sprite=filesystemType
					};
				tileMarkers.Add(newMarkerType);
			}
			return tileMarkers;
		}
			
	}
}

