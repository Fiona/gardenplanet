using UnityEngine;

namespace StompyBlondie
{
    public static class TransformHelper
    {
        /*
         * Searches the child heirarchy of the given transform for the name given.
         */
        public static Transform FindRecursive(this Transform parent, string name)
        {
            if(parent.name == name)
                return parent;
            foreach(Transform child in parent)
            {
                var findRecurse = child.FindRecursive(name);
                if(findRecurse != null)
                    return findRecurse;
            }
            return null;
        }

    }
}