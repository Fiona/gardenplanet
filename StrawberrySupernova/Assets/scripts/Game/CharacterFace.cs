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

            if(!String.IsNullOrEmpty(appearence.faceDetail1))
            {
                var faceDetail1Texture = Resources.Load<Texture>(Consts.CHARACTERS_FACE_DETAILS_TEXTURE_PATH + appearence.faceDetail1);
                if(faceDetail1Texture!= null)
                {
                    Graphics.Blit(faceDetail1Texture, renderTexture, facePartMaterial);
                }
                else
                    Debug.Log(String.Format("Can't find face detail resource {0}", appearence.faceDetail1));
            }

            if(!String.IsNullOrEmpty(appearence.faceDetail2))
            {
                var faceDetail2Texture = Resources.Load<Texture>(Consts.CHARACTERS_FACE_DETAILS_TEXTURE_PATH + appearence.faceDetail2);
                if(faceDetail2Texture!= null)
                {
                    Graphics.Blit(faceDetail2Texture, renderTexture, facePartMaterial);
                }
                else
                    Debug.Log(String.Format("Can't find face detail resource {0}", appearence.faceDetail2));
            }

            var mouthTexture = Resources.Load<Texture>(Consts.CHARACTERS_MOUTHS_TEXTURE_PATH + appearence.mouth);
            if(mouthTexture != null)
            {
                Graphics.Blit(mouthTexture, renderTexture, facePartMaterial);
            }
            else
                Debug.Log(String.Format("Can't find mouth resource {0}", appearence.mouth));

            var noseTexture = Resources.Load<Texture>(Consts.CHARACTERS_NOSES_TEXTURE_PATH + appearence.nose);
            if(noseTexture != null)
            {
                Graphics.Blit(noseTexture, renderTexture, facePartMaterial);
            }
            else
                Debug.Log(String.Format("Can't find nose resource {0}", appearence.nose));

            var eyesTexture = Resources.Load<Texture2D>(Consts.CHARACTERS_EYES_TEXTURE_PATH + appearence.eyes);
            var eyesTextureTint = Resources.Load<Texture2D>(Consts.CHARACTERS_EYES_TEXTURE_PATH + appearence.eyes + "_tint");
            if(eyesTexture != null)
            {
                facePartMaterial.SetColor("_Color", appearence.eyeColor);
                Graphics.Blit(eyesTextureTint, renderTexture, facePartMaterial);
                facePartMaterial.SetColor("_Color", Color.white);
                Graphics.Blit(eyesTexture, renderTexture, facePartMaterial);
            }
            else
                Debug.Log(String.Format("Can't find eyes resource {0}", appearence.eyes));

            var eyebrowsTexture = Resources.Load<Texture>(Consts.CHARACTERS_EYEBROWS_TEXTURE_PATH + appearence.eyebrows);
            if(eyebrowsTexture != null)
            {
                Graphics.Blit(eyebrowsTexture, renderTexture, facePartMaterial);
            }
            else
                Debug.Log(String.Format("Can't find eyebrows resource {0}", appearence.eyebrows));

        }

    }
}