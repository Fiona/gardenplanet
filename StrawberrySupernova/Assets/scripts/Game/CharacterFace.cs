using System;
using StompyBlondie;
using UnityEngine;

namespace StrawberryNova
{
    public class CharacterFace: MonoBehaviour
    {
        private RenderTexture renderTexture;
        private Material faceMaterial;
        private Character.Appearence appearence;
        private bool suppliedAppearence;

        private void Awake()
        {
            renderTexture = new RenderTexture(2048, 2048, 16, RenderTextureFormat.ARGB32);
            renderTexture.Create();
        }

        private void Start()
        {
            faceMaterial = GetComponentInChildren<SkinnedMeshRenderer>().material;
            faceMaterial.mainTexture = renderTexture;
        }

        private void Update()
        {
            if(!renderTexture.IsCreated())
                Recreate();
        }

        private void OnDestroy()
        {
            renderTexture.Release();
            renderTexture = null;
        }

        public void Recreate()
        {
            if(suppliedAppearence)
                Recreate(appearence);
        }

        public void Recreate(Character.Appearence _appearence)
        {
            suppliedAppearence = true;
            appearence = _appearence;
            renderTexture.Clear(Color.clear);

            var facePartMaterial = new Material(Shader.Find("Hidden/FacePart"));

            // face detail
            var detail1Color = new Color(1f, 1f, 1f, appearence.faceDetail1Opacity);
            BlitFacePart(Consts.CHARACTERS_FACE_DETAILS_TEXTURE_PATH, appearence.faceDetail1, renderTexture,
                facePartMaterial, detail1Color, appearence.faceDetail1FlipHorizontal);
            var detail2Color = new Color(1f, 1f, 1f, appearence.faceDetail2Opacity);
            BlitFacePart(Consts.CHARACTERS_FACE_DETAILS_TEXTURE_PATH, appearence.faceDetail2, renderTexture,
                facePartMaterial, detail2Color, appearence.faceDetail2FlipHorizontal);

            // mouth
            BlitFacePart(Consts.CHARACTERS_MOUTHS_TEXTURE_PATH, appearence.mouth, renderTexture, facePartMaterial);

            // nose
            BlitFacePart(Consts.CHARACTERS_NOSES_TEXTURE_PATH, appearence.nose, renderTexture, facePartMaterial);

            // eyes
            var doEyeTint = BlitFacePart(Consts.CHARACTERS_EYES_TEXTURE_PATH, appearence.eyes + "_tint", renderTexture,
                facePartMaterial, appearence.eyeColor);
            if(doEyeTint)
            {
                BlitFacePart(Consts.CHARACTERS_EYES_TEXTURE_PATH, appearence.eyes, renderTexture,
                    facePartMaterial);
            }

            // eyesbrows
            BlitFacePart(Consts.CHARACTERS_EYEBROWS_TEXTURE_PATH, appearence.eyebrows, renderTexture, facePartMaterial);
        }

        private bool BlitFacePart(string resourcePath, string resourceName, RenderTexture destination,
            Material material, Color? color = null, bool flipHorizontally = false)
        {
            if(String.IsNullOrEmpty(resourceName))
                return false;

            var texture = Resources.Load<Texture>(resourcePath + resourceName);
            if(texture == null)
            {
                Debug.Log(String.Format("Can't find face part resource {0}", resourcePath + resourceName));
                return false;
            }

            material.SetFloat("_FlipHorizontal", flipHorizontally ? 1f : 0f);
            material.SetColor("_Color", color.GetValueOrDefault(Color.white));
            Graphics.Blit(texture, destination, material);
            return true;
        }

    }
}