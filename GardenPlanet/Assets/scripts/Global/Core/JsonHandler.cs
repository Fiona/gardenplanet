using System;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GardenPlanet
{

    public class AttributesConverver : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var jObject = new JObject();
            var attrs = (Attributes)value;
            foreach(var attr in attrs)
                jObject.Add(attr.Key.ToString(), JToken.FromObject(attr.Value, serializer));
            jObject.WriteTo(writer);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var attributes = new Attributes();

            JObject obj = (JObject)serializer.Deserialize(reader);
            if(obj == null)
                return attributes;
            foreach(var prop in obj.Properties())
            {
                switch(prop.Value.Type)
                {
                    case JTokenType.Boolean:
                        attributes.Add(prop.Name, (bool)prop.Value);
                        break;
                    case JTokenType.Integer:
                        attributes.Add(prop.Name, (int)prop.Value);
                        break;
                    case JTokenType.Float:
                        attributes.Add(prop.Name, (float)prop.Value);
                        break;
                    case JTokenType.String:
                        attributes.Add(prop.Name, (string)prop.Value);
                        break;
                    default:
                        attributes.Add(prop.Name, prop.Value);
                        break;
                }
            }

            return attributes;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Attributes);
        }
    }

    public static class JsonHandler
    {
        public static T Deserialize<T>(string jsonContents)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(jsonContents, GetSerializerSettings());
            }
            catch(JsonException e)
            {
                throw new JsonErrorException(e.Message, e);
            }
        }

        public static string Serialize(object obj)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, GetSerializerSettings());
            }
            catch(JsonException e)
            {
                throw new JsonErrorException(e.Message, e);
            }
        }

        public static void SerializeToFile(object obj, string filePath)
        {
            File.WriteAllText(filePath, Serialize(obj));
        }

        public static void PopulateObject(string json, object obj)
        {
            JsonConvert.PopulateObject(json, obj, GetSerializerSettings());
        }

        private static JsonSerializerSettings GetSerializerSettings()
        {
            var serializerSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented
            };
            serializerSettings.Converters.Add(new AttributesConverver());
            return serializerSettings;
        }
    }

}