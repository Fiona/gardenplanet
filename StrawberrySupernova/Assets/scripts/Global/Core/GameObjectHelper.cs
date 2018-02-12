using UnityEngine;

namespace StompyBlondie
{
    public static class GameObjectHelper
    {
        public static void DestroyAllChildren(this GameObject parent)
        {
            foreach(Transform child in parent.transform)
                Object.Destroy(child.gameObject);
        }

        public static void DestroyAllChildrenImmediate(this GameObject parent)
        {
            foreach(Transform child in parent.transform)
                Object.DestroyImmediate(child.gameObject);
        }
    }
}