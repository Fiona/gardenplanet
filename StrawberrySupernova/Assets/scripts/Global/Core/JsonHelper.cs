using System;
using System.Collections.Generic;
using LitJson;

namespace StompyBlondie
{
    [Serializable]
    public class ListWrapper<T>
    {
        public List<T> Items;
    }

    public static class JsonHelper
    {
        public static List<T> ToList<T>(string json)
        {
            var wrapper = JsonMapper.ToObject<ListWrapper<T>>(json);
            return wrapper.Items;
        }
    }
}