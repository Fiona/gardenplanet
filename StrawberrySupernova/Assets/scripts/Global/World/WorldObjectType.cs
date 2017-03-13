using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace StrawberryNova
{
	public class WorldObjectType
	{

		public struct WorldObjectDataEntry
		{
			public string displayName;
			public bool interactable;
		};

		public struct WorldObjectTypeDataFile
		{
			public Dictionary<string, WorldObjectDataEntry> worldObjectTypes;
		};
			
		public string name;
		public GameObject prefab;
		public WorldObjectDataEntry data;

		public string displayName
		{
			get{ return data.displayName; }
			set{ data.displayName = value; }
		}
		public bool interactable
		{
			get{ return data.interactable; }
			set{ data.interactable = value; }
		}

		/*
		 * Returns a List containing all available world objects.
		 */
		public static List<WorldObjectType> GetAllWorldObjectTypes()
		{
			// Get all data on world objects
			var worldObjectData = new Dictionary<string, WorldObjectDataEntry>();
			var dirPath = Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_WORLD_OBJECT_DATA);
			if(Directory.Exists(dirPath))
			{
				var dataFiles = Directory.GetFiles(dirPath, "*." + Consts.FILE_EXTENSION_WORLD_OBJECT_DATA);
				foreach(var f in dataFiles)
				{
					try
					{
						using(var fh = File.OpenText(f))
						{
							var loadedDataFile = JsonMapper.ToObject<WorldObjectTypeDataFile>(fh.ReadToEnd());
							foreach(var singleObjectData in loadedDataFile.worldObjectTypes)
							{
								worldObjectData.Add(singleObjectData.Key,
									new WorldObjectDataEntry
									{
										displayName=singleObjectData.Value.displayName,
										interactable=singleObjectData.Value.interactable
									}
								);
							}
						}
					}
					catch(JsonException e)
					{
						Debug.Log(e);
					}
				}
			}

			// Get and create all objects
			var allObjects = new List<WorldObjectType>();
			var prefabs = Resources.LoadAll<GameObject>("worldobjects/");
			foreach(var objectType in prefabs)
			{				
				var newObjectType = new WorldObjectType
					{
						name=objectType.name,
						prefab=(GameObject)objectType
					};
				if(worldObjectData.ContainsKey(objectType.name))
					newObjectType.data = worldObjectData[objectType.name];
				allObjects.Add(newObjectType);
			}
			// Reorder by name
			allObjects.Sort((x, y) => x.name.CompareTo(y.name));
			return allObjects;
		}

		public static WorldObjectTypeDataFile CreateEmptyWorldObjectDataFile(string name)
		{

			var newDataFile = new WorldObjectTypeDataFile {
				worldObjectTypes=new Dictionary<string, WorldObjectDataEntry>()
			};
			newDataFile.worldObjectTypes.Add("nothing01",
				new WorldObjectDataEntry
				{
					displayName="Completely Nothing",
					interactable=false
				}
			);

			var filepath = GetWorldObjectDataFilePathFromName(name);		

			// Check directories exist
			if(!Directory.Exists(Consts.DATA_DIR))
				Directory.CreateDirectory(Consts.DATA_DIR);
			if(!Directory.Exists(Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_MAPS)))
				Directory.CreateDirectory(Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_MAPS));

			var jsonOutput = new StringBuilder();
			var writer = new JsonWriter(jsonOutput);
			writer.PrettyPrint = true;
			JsonMapper.ToJson(newDataFile, writer);

			using(var fh = File.OpenWrite(filepath))
			{
				var jsonBytes = Encoding.UTF8.GetBytes(jsonOutput.ToString());
				fh.SetLength(0);
				fh.Write(jsonBytes, 0, jsonBytes.Length);
			}

			return newDataFile;

		}

		public static string GetWorldObjectDataFilePathFromName(string name)
		{
			return Path.Combine(
				Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_WORLD_OBJECT_DATA),
				String.Format("{0}.{1}", name, Consts.FILE_EXTENSION_WORLD_OBJECT_DATA)
			);
		}

	}
}