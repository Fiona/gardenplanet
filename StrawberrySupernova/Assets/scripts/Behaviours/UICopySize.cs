using UnityEngine;

namespace StompyBlondie
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    public class UICopySize : MonoBehaviour
    {

        public RectTransform objectToCopy;
        public float padding;
        private RectTransform myTransform;

        public void Start()
        {
            myTransform = GetComponent<RectTransform>();
        }

        public void LateUpdate()
        {
            if(objectToCopy != null)
                myTransform.sizeDelta = objectToCopy.sizeDelta + new Vector2(padding, padding);
        }

    }
}