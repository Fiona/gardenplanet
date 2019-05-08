using System;
using System.Collections;
using System.Collections.Generic;
using StompyBlondie;
using UnityEngine;

namespace GardenPlanet
{
    public class CharacterFace: MonoBehaviour
    {
        public enum FaceState
        {
            NORMAL,
            EYES_CLOSED
        };

        private Dictionary<FaceState, RenderTexture> renderTextures;
        private Material faceMaterial;
        private Character.Appearence appearence;
        private bool suppliedAppearence;
        private FaceState currentState = FaceState.NORMAL;
        private float blinkWait;
        private float blinkTime;

        private void Awake()
        {
            renderTextures = new Dictionary<FaceState, RenderTexture>();
            foreach(var state in Enum.GetValues(typeof(FaceState)))
            {
                var newRenderTexture = new RenderTexture(2048, 2048, 16, RenderTextureFormat.ARGB32);
                newRenderTexture.Create();
                renderTextures[(FaceState)state] = newRenderTexture;
            }
        }

        private void Start()
        {
            faceMaterial = GetComponentInChildren<SkinnedMeshRenderer>().material;
            SetFaceState(FaceState.NORMAL);
            StartCoroutine(DoExpressions());
        }

        private void Update()
        {
            if(!renderTextures[FaceState.NORMAL].IsCreated())
            {
                Recreate();
                return;
            }
        }

        private IEnumerator DoExpressions()
        {
            while(true)
            {
                if(currentState == FaceState.EYES_CLOSED)
                {
                    SetRenderTexture(renderTextures[FaceState.EYES_CLOSED]);
                }
                else if(currentState == FaceState.NORMAL)
                {
                    SetRenderTexture(renderTextures[FaceState.NORMAL]);
                    var betweenBlinkTime = UnityEngine.Random.Range(Consts.CHARACTER_BETWEEN_BLINK_WAIT_RANGE[0],
                        Consts.CHARACTER_BETWEEN_BLINK_WAIT_RANGE[1]);
                    yield return new WaitForSeconds(betweenBlinkTime);
                    SetRenderTexture(renderTextures[FaceState.EYES_CLOSED]);
                    var blinkTime = UnityEngine.Random.Range(Consts.CHARACTER_BLINK_TIME_RANGE[0],
                        Consts.CHARACTER_BLINK_TIME_RANGE[1]);
                    yield return new WaitForSeconds(blinkTime);
                }
                yield return new WaitForFixedUpdate();
            }
        }

        private void OnDestroy()
        {
            foreach(var texture in renderTextures.Values)
                texture.Release();
            renderTextures = null;
        }

        public void Recreate()
        {
            if(suppliedAppearence)
                Recreate(appearence);
        }

        public void Recreate(Character.Appearence _appearence)
        {
            appearence = _appearence;
            foreach(var state in renderTextures.Keys)
                GenerateFaceTexture(renderTextures[state], state);
        }

        public void SetFaceState(FaceState state)
        {
            currentState = state;
            faceMaterial.SetTexture("_MainTex", renderTextures[state]);
        }

        private void SetRenderTexture(RenderTexture setTo)
        {
            if(setTo.IsCreated())
                faceMaterial.SetTexture("_BaseColorMap", setTo);
        }

        private void GenerateFaceTexture(RenderTexture texture, FaceState state)
        {
            suppliedAppearence = true;
            texture.Clear(Color.clear);

            var facePartMaterial = new Material(Shader.Find("Hidden/FacePart"));

            // face detail
            var detail1Color = new Color(1f, 1f, 1f, appearence.faceDetail1Opacity);
            BlitFacePart(Consts.CHARACTERS_FACE_DETAILS_TEXTURE_PATH, appearence.faceDetail1, texture,
                facePartMaterial, detail1Color, appearence.faceDetail1FlipHorizontal);
            var detail2Color = new Color(1f, 1f, 1f, appearence.faceDetail2Opacity);
            BlitFacePart(Consts.CHARACTERS_FACE_DETAILS_TEXTURE_PATH, appearence.faceDetail2, texture,
                facePartMaterial, detail2Color, appearence.faceDetail2FlipHorizontal);

            // mouth
            BlitFacePart(Consts.CHARACTERS_MOUTHS_TEXTURE_PATH, appearence.mouth, texture, facePartMaterial);

            // nose
            BlitFacePart(Consts.CHARACTERS_NOSES_TEXTURE_PATH, appearence.nose, texture, facePartMaterial);

            // eyes
            if(state == FaceState.EYES_CLOSED)
            {
                BlitFacePart(Consts.CHARACTERS_EYES_TEXTURE_PATH, appearence.eyes + "_closed", texture,
                    facePartMaterial);
            }
            else
            {
                var doEyeTint = BlitFacePart(Consts.CHARACTERS_EYES_TEXTURE_PATH, appearence.eyes + "_tint", texture,
                    facePartMaterial, appearence.eyeColor);
                if(doEyeTint)
                {
                    BlitFacePart(Consts.CHARACTERS_EYES_TEXTURE_PATH, appearence.eyes, texture,
                        facePartMaterial);
                }
            }

            // eyesbrows
            BlitFacePart(Consts.CHARACTERS_EYEBROWS_TEXTURE_PATH, appearence.eyebrows, texture, facePartMaterial);
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