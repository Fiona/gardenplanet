using System.CodeDom;
using System.Collections.Generic;
using UnityEngine;

namespace StompyBlondie
{
    public class BoneClone : MonoBehaviour
    {
        public SkinnedMeshRenderer rendererToClone;

        private void Start()
        {
            var boneMap = new Dictionary<string, Transform>();
            foreach(var bone in rendererToClone.bones)
                boneMap[bone.name] = bone;

            var thisRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            var boneArray = thisRenderer.bones;
            var i = 0;
            foreach(var b in boneArray)
            {
                var boneName = b.name;
                Transform store;
                if(boneMap.TryGetValue(boneName, out store) == false)
                {
                    Debug.LogError("failed to get bone: " + boneName);
                    continue;
                }
                boneArray[i] = store;
                i++;
            }

            thisRenderer.rootBone = rendererToClone.rootBone;
            thisRenderer.bones = boneArray;
        }
    }
}