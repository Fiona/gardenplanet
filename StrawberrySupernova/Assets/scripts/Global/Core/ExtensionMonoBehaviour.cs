using UnityEngine;

namespace Global.Core
{
    public class ExtensionMonoBehaviour: MonoBehaviour
    {
        private static ExtensionMonoBehaviour instance;

        public static ExtensionMonoBehaviour GetInstance()
        {
            if(instance == null)
            {
                var obj = new GameObject("Extension MonoBehaviour");
                instance = obj.AddComponent<ExtensionMonoBehaviour>();
            }
            return instance;
        }
    }
}