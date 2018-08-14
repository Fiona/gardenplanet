using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GardenPlanet
{
    public class Attributes: IEnumerable<DictionaryEntry>
    {

        private Hashtable items;

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
                    items[entry.Key] = entry.Value;
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
         * Set the value given to the key given.
         */
        public void Set<T>(string key, T val)
        {
            items[key] = val;
        }

        /*
         * If the Attributes contain a particular key
         */
        public bool Contains(string key)
        {
            return items.Contains(key);
        }

        /*
         * Removes the specified key from the attributes
         */
        public void Remove(string key)
        {
            if(items[key] == null)
                throw new KeyNotFoundException($"Key {key} not found in Attributes");
            items.Remove(key);
        }

        /*
         * Checks if the key passed is a particular type
         */
        public bool IsKeyType(string key, Type type)
        {
            var item = items[key];
            if(item == null)
                throw new KeyNotFoundException($"Key {key} not found in Attributes");
            return item.GetType() == type;
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