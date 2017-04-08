using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace StrawberryNova
{
    public class ItemType
    {

        public struct ItemTypeData
        {
            public string ID;
            public string DisplayName;
            public string Description;
            public string Category;
            public int StackSize;
            public bool CanPickup;
        }

        public struct ItemTypeDataFile
        {
            public Dictionary<string, ItemTypeData> itemTypes;
        };

        public ItemTypeData data;

        public string ID
        {
            get{ return data.ID; }
            set{ data.ID = value; }
        }

        public string DisplayName
        {
            get{ return data.DisplayName; }
            set{ data.DisplayName = value; }
        }

        public string Description
        {
            get{ return data.Description; }
            set{ data.Description = value; }
        }

        public string Category
        {
            get{ return data.Category; }
            set{ data.Category = value; }
        }

        public int StackSize
        {
            get{ return data.StackSize; }
            set{ data.StackSize = value; }
        }

        public bool CanPickup
        {
            get{ return data.CanPickup; }
            set{ data.CanPickup = value; }
        }

        /*
         * Returns a List containing all items
         */
        public static List<ItemType> GetAllItemTypes()
        {
            // Get all data on item types
            var itemTypeData = new Dictionary<string, ItemTypeData>();
            var dirPath = Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_ITEM_TYPE_DATA);
            if(Directory.Exists(dirPath))
            {
                var dataFiles = Directory.GetFiles(dirPath, "*." + Consts.FILE_EXTENSION_ITEM_TYPE_DATA);
                foreach(var f in dataFiles)
                {
                    try
                    {
                        using(var fh = File.OpenText(f))
                        {
                            var loadedDataFile = JsonMapper.ToObject<ItemTypeDataFile>(fh.ReadToEnd());
                            foreach(var singleItemTypeData in loadedDataFile.itemTypes)
                            {
                                itemTypeData.Add(singleItemTypeData.Key,
                                    new ItemTypeData
                                    {
                                        ID=singleItemTypeData.Value.ID,
                                        DisplayName=singleItemTypeData.Value.DisplayName,
                                        Description=singleItemTypeData.Value.Description,
                                        Category=singleItemTypeData.Value.Category,
                                        StackSize=singleItemTypeData.Value.StackSize,
                                        CanPickup=singleItemTypeData.Value.CanPickup
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

            // Collate into list
            var allItems = new List<ItemType>();
            foreach(var i in itemTypeData)
            {
                allItems.Add(
                    new ItemType {
                        data = i.Value
                    }
                );
            }

            // Reorder and return
            allItems.Sort((x, y) => x.Category.CompareTo(y.Category));
            return allItems;
        }

        public static ItemTypeDataFile CreateEmptyItemTypeDataFile(string name)
        {

            var newDataFile = new ItemTypeDataFile {
                itemTypes=new Dictionary<string, ItemTypeData>()
            };
            newDataFile.itemTypes.Add("bad_wolf",
                new ItemTypeData
                {                   
                    ID= "bad_wolf",
                    DisplayName="Bad Wolf",
                    Description="Not really a wolf nor bad. Just a boring Dr Who reference.",
                    Category="boop",
                    StackSize=64,
                    CanPickup=true
                }
            );

            var filepath = GetItemTypeDataFilePathFromName(name);       

            // Check directories exist
            if(!Directory.Exists(Consts.DATA_DIR))
                Directory.CreateDirectory(Consts.DATA_DIR);
            if(!Directory.Exists(Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_ITEM_TYPE_DATA)))
                Directory.CreateDirectory(Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_ITEM_TYPE_DATA));

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
            
        public static string GetItemTypeDataFilePathFromName(string name)
        {
            return Path.Combine(
                Path.Combine(Consts.DATA_DIR, Consts.DATA_DIR_ITEM_TYPE_DATA),
                String.Format("{0}.{1}", name, Consts.FILE_EXTENSION_ITEM_TYPE_DATA)
            );
        }

    }
}

