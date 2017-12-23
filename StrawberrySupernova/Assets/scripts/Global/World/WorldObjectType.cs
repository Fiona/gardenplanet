using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
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
			public bool tileObject;
			public bool hideInEditor;
			public string script;
			public Hashtable defaultAttributes;
		};

		public struct WorldObjectTypeDataFile
		{
			public Dictionary<string, WorldObjectDataEntry> worldObjectTypes;
		};

		public string name;
		public GameObject prefab;

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
		public bool tileObject
		{
			get{ return data.tileObject; }
			set{ data.tileObject = value; }
		}
		public bool hideInEditor
		{
			get{ return data.hideInEditor; }
			set{ data.hideInEditor = value; }
		}
		public string script
		{
			get{ return data.script; }
			set{ data.script = value; }
		}
		public Hashtable defaultAttributes
		{
			get{ return data.defaultAttributes; }
			set{ data.defaultAttributes = value; }
		}

		private WorldObjectDataEntry data;

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
							var fileConents = fh.ReadToEnd();
							fileConents = Regex.Replace(fileConents, @"\/\*(.*)\*\/", String.Empty);
							var loadedDataFile = JsonMapper.ToObject<WorldObjectTypeDataFile>(fileConents);
							foreach(var singleObjectData in loadedDataFile.worldObjectTypes)
							{
								worldObjectData.Add(singleObjectData.Key,
									new WorldObjectDataEntry
									{
										displayName=singleObjectData.Value.displayName,
										interactable=singleObjectData.Value.interactable,
										tileObject=singleObjectData.Value.tileObject,
										hideInEditor=singleObjectData.Value.hideInEditor,
										script=singleObjectData.Value.script,
										defaultAttributes=singleObjectData.Value.defaultAttributes
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
			var prefabs = Resources.LoadAll<GameObject>(Consts.WORLD_OBJECTS_PREFABS_PATH);
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
            if(!Directory.Exists(Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_WORLD_OBJECT_DATA)))
                Directory.CreateDirectory(Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_WORLD_OBJECT_DATA));

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