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

        public static void SetLayerRecursively(this GameObject obj, int layer)
        {
            obj.layer = layer;

            foreach(Transform child in obj.transform)
                child.gameObject.SetLayerRecursively(layer);
        }

        public static bool HasComponent<T>(this GameObject obj) where T : Component
        {
            return obj.GetComponent<T> () != null;
        }
    }
}