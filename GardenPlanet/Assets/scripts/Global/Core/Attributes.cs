using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LitJson;
using UnityEditor;
using UnityEngine;

namespace GardenPlanet
{
    public class Attributes: IEnumerable<DictionaryEntry>
    {

        public Hashtable items;

        public int Count
        {
            get { return items.Count; }
        }

        /*
         * Constructor with no defaults
         */
        public Attributes()
        {
            items = new Hashtable();
        }

        /*
         * Constructor with Hashtable object as default
         */
        public Attributes(Hashtable initial)
        {
            items = new Hashtable();
            if(initial != null && initial.Count > 0)
                foreach(DictionaryEntry entry in initial)
                    items[entry.Key] = ConvertFromJsonData(entry.Value);
        }

        /*
         * Constructor with other Attributes objects
         */
        public Attributes(Attributes initial)
        {
            items = new Hashtable();
            if(initial != null && initial.Count > 0)
                foreach(DictionaryEntry entry in initial.items)
                    items[entry.Key] = entry.Value;
        }

        /*
         * summary: Gets a value of the key and type given. Will give a float even if an int is stored and vise-versa.
         * parameters:
         *     string key: The key of the value requested.
         * returns: The value of type requested.
         * throws:
         *     KeyNotFoundException: When the key specified has not been set.
         *     InvalidCastException: If the value cannot be coorced into the type passed.
         */
        public T Get<T>(string key)
        {
            var item = items[key];
            if(item == null)
                throw new KeyNotFoundException($"Key {key} not found in Attributes");

            var typeOfItem = item.GetType();
            var wantType = typeof(T);

            // Int in, want float
            if(typeOfItem == typeof(int) && wantType == typeof(float))
                return (T)Convert.ChangeType((int)items[key], wantType);
            // Float in, want int
            if(typeOfItem == typeof(float) && wantType == typeof(int))
                return (T)Convert.ChangeType((float)items[key], wantType);

            return (T)items[key];
        }

        /*
         * Set the value given to the key given. If a jsondata object is passed this will automatically be
         * turned into a primitive before storing.
         */
        public void Set<T>(string key, T val)
        {
            items[key] = ConvertFromJsonData(val);
        }

        /*
         * Turns a json data object into either bool, string, int or float depending on what it is.
         */
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

        /*
         * Helper method for returning if the jsondata object is a float, int, whatever
         */
        private static bool IsNumeric(JsonData jsonData)
        {
            return jsonData.IsDouble || jsonData.IsInt || jsonData.IsLong;
        }

        /*
         * Helper method for getting a float value out of the jsondata
         */
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

        /*
         * Equality overload
         */
        public override bool Equals(System.Object obj)
        {
            var attributes = obj as Attributes;
            return this == attributes;
        }

        public override int GetHashCode()
        {
            return items.GetHashCode();
        }

        public static bool operator ==(Attributes x, Attributes y)
        {
            // Null checks
            if(ReferenceEquals(x, null))
            {
                if(ReferenceEquals(y, null))
                    return true;
                return y.Count == 0;
            }

            if(ReferenceEquals(y, null))
                return x.Count == 0;
            // Count checks
            if(x.Count() != y.Count())
                return false;
            // Compare each item in the objects
            foreach(DictionaryEntry i in x.items)
            {
                if(!y.items.ContainsKey(i.Key))
                    return false;
                if(!((object)y.items[i.Key]).Equals((object)i.Value))
                    return false;
            }
            return true;
        }

        public static bool operator !=(Attributes x, Attributes y)
        {
            return !(x == y);
        }

        public static Attributes operator +(Attributes x, Attributes y)
        {
            // Null checks
            if(ReferenceEquals(x, null))
                return new Attributes(y);
            if(ReferenceEquals(y, null))
                return new Attributes(x);
            // Build new attrs with first list as base
            var concatAttributes = new Attributes(x);
            // Override with second list
            foreach(var item in y)
                concatAttributes.items[item.Key] = item.Value;
            return concatAttributes;
        }

        /*
         * Enumerable inplementation
         */
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(string key, object value)
        {
            items[key] = value;
        }

        public IEnumerator<DictionaryEntry> GetEnumerator()
        {
            // This somehow made it work cool
            var list = new List<DictionaryEntry>();
            foreach(DictionaryEntry i in items)
                list.Add(i);
            return list.GetEnumerator();
        }

        public override string ToString()
        {
            var repr = new List<string>();
            foreach(DictionaryEntry i in items)
                repr.Add($"{i.Key}: {i.Value}");
            return $"Attributes ({Count}): [{string.Join(", ", repr)}]";
        }

    }
}