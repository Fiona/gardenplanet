using System;
using System.Collections.Generic;

namespace StompyBlondie
{
    public static class EnumerableMap
    {
        public static IEnumerable<U> Map<T, U>(this IEnumerable<T> s, Func<T, U> f)
        {
            foreach (var item in s)
                yield return f(item);
        }
    }
}