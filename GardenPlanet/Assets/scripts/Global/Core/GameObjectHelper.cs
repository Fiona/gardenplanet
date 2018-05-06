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

        public static void EnableAllColliders(this GameObject obj)
        {
            SetCollidersEnabled(obj, true);
        }

        public static void DisableAllColliders(this GameObject obj)
        {
            SetCollidersEnabled(obj, false);
        }

        private static void SetCollidersEnabled(GameObject obj, bool set)
        {
            var cols = obj.GetComponentsInChildren<Collider>();
            foreach(var col in cols)
                col.enabled = set;
        }


    }
}