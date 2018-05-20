using System;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using UnityEngine;

namespace GardenPlanet
{
    public class Attributes
    {

        private Hashtable items;

        public Attributes(Hashtable initial = null)
        {
            items = new Hashtable();
            if(initial != null && initial.Count > 0)
                foreach(DictionaryEntry entry in initial)
                    items[entry.Key] = ConvertFromJsonData(entry.Value);
        }

        public T Get<T>(string key)
        {
            var typeOfItem = items[key].GetType();
            var wantType = typeof(T);

            // Int in, want float
            if(typeOfItem == typeof(int) && wantType == typeof(float))
                return (T)Convert.ChangeType((int)items[key], wantType);
            // Float in, want int
            if(typeOfItem == typeof(float) && wantType == typeof(int))
                return (T)Convert.ChangeType((float)items[key], wantType);

            return (T)items[key];
        }

        public void Set<T>(string key, T val)
        {
            items[key] = ConvertFromJsonData(val);
        }

        private object ConvertFromJsonData(object val)
        {
            var jsonData = val as JsonData;
            if(jsonData != null)
            {
                if(jsonData.IsBoolean)
                    return (bool)jsonData;
                if(jsonData.IsString)
                    return (string)jsonData;
                if(IsNumeric(jsonData))
                {
                    var floatData = CastToFloat(jsonData);
                    if(Math.Abs(floatData % 1.0f) > 0.001f)
                        return floatData;
                    return (int)floatData;
                }
            }
            return val;
        }

        private static bool IsNumeric(JsonData jsonData)
        {
            return jsonData.IsDouble || jsonData.IsInt || jsonData.IsLong;
        }

        private static float CastToFloat(JsonData jsonData)
        {
            switch(jsonData.GetJsonType())
            {
                case JsonType.Int:
                    return (int)jsonData;
                case JsonType.Long:
                    return (long)jsonData;
                case JsonType.Double:
                    return (float)(double)jsonData;
                default:
                    throw new ArgumentException($"Non-numeric json type: {jsonData.GetJsonType()}");
            }
        }

    }
}