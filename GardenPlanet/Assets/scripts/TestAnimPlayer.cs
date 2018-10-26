using UnityEngine;
using StompyBlondie;

namespace GardenPlanet
{
    public class TestAnimPlayer: MonoBehaviour
    {
        private GameObject visualsHolder;
        private GameObject baseModel;

        private void Start()
        {
            visualsHolder = new GameObject("visuals");
            visualsHolder.transform.SetParent(transform, false);

            baseModel = AddModelToVisuals(Consts.CHARACTERS_BASE_MODEL_VISUAL_PATH + "basemodel");
            var bonesToClone = baseModel.GetComponentInChildren<SkinnedMeshRenderer>();
            AddModelToVisuals(Consts.CHARACTERS_BASE_MODEL_VISUAL_PATH + "basemodel_face", bonesToClone);
            //AddModelToVisuals(Consts.CHARACTERS_TOPS_VISUAL_PATH + "ilovefarmingshirt", bonesToClone);

            Animator anim = GetComponent<Animator>();
            anim.SetBool("DoWalk", true);
        }

        private GameObject AddModelToVisuals(string prefabPath, SkinnedMeshRenderer bonesToClone = null)
        {
            var resource = Resources.Load(prefabPath) as GameObject;
            if(resource == null)
            {
                Debug.Log("Can't find visuals model " + prefabPath);
                return null;
            }

            var newObject = Instantiate(resource);
            newObject.SetLayerRecursively(Consts.COLLISION_LAYER_CHARACTERS);
            newObject.transform.SetParent(visualsHolder.transform, false);

            if(bonesToClone != null)
            {
                var boneClone = newObject.AddComponent<BoneClone>();
                boneClone.rendererToClone = bonesToClone;
            }

            return newObject;
        }

    }
}