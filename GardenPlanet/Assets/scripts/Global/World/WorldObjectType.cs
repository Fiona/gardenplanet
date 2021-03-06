using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;


namespace GardenPlanet
{

    public class WorldObjectType
    {

        public class WorldObjectDataEntry
        {
            public string displayName;
            public bool interactable;
            public bool tileObject;
            public bool hideInEditor;
            public bool ghost;
            public string script;
            public Attributes defaultAttributes;
        };

        public class WorldObjectTypeDataFile
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
        public bool ghost
        {
            get{ return data.ghost; }
            set{ data.ghost = value; }
        }
        public string script
        {
            get{ return data.script; }
            set{ data.script = value; }
        }
        public Attributes defaultAttributes
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
            var worldObjects = new List<WorldObjectType>();
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

                            var loadedDataFile = JsonHandler.Deserialize<WorldObjectTypeDataFile>(fileConents);
                            foreach(var singleObjectData in loadedDataFile.worldObjectTypes)
                            {
                                var newType = new WorldObjectType
                                {
                                    name=singleObjectData.Key,
                                    data=new WorldObjectDataEntry
                                    {
                                        displayName=singleObjectData.Value.displayName,
                                        interactable=singleObjectData.Value.interactable,
                                        tileObject=singleObjectData.Value.tileObject,
                                        hideInEditor=singleObjectData.Value.hideInEditor,
                                        ghost=singleObjectData.Value.ghost,
                                        script=singleObjectData.Value.script,
                                        defaultAttributes=singleObjectData.Value.defaultAttributes
                                    }
                                };
                                if(newType.defaultAttributes == null)
                                    newType.defaultAttributes = new Attributes();
                                newType.prefab = Resources.Load<GameObject>(
                                    Path.Combine(Consts.WORLD_OBJECTS_PREFABS_PATH, singleObjectData.Key)
                                );
                                worldObjects.Add(newType);
                            }
                        }
                    }
                    catch(JsonErrorException e)
                    {
                        Debug.Log(e);
                    }
                }
            }

            // Reorder by name
            worldObjects.Sort((x, y) => x.name.CompareTo(y.name));
            return worldObjects;
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
                    interactable=false,
                    defaultAttributes=new Attributes{{"foo", "bar"}}
                }
            );

            var filepath = GetWorldObjectDataFilePathFromName(name);

            // Check directories exist
            if(!Directory.Exists(Consts.DATA_DIR))
                Directory.CreateDirectory(Consts.DATA_DIR);
            if(!Directory.Exists(Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_WORLD_OBJECT_DATA)))
                Directory.CreateDirectory(Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_WORLD_OBJECT_DATA));

            JsonHandler.SerializeToFile(newDataFile, filepath);

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