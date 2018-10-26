using System;
using UnityEngine;

namespace GardenPlanet
{
    public class CACCharacter : Character
    {
        public void Start()
        {
            rigidBody.freezeRotation = false;
            //RegenerateVisuals();
            //RegenerateFace();
        }

        public override void FixedUpdate()
        {
        }

        public void SetAppearenceValue(string appearenceType, string appearenceValue)
        {
            switch(appearenceType)
            {
                case "eyes":
                    SetEyes(appearenceValue);
                    break;
                case "eyebrows":
                    SetEyebrows(appearenceValue);
                    break;
                case "noses":
                    SetNose(appearenceValue);
                    break;
                case "mouths":
                    SetMouth(appearenceValue);
                    break;
                case "faceDetail1":
                    SetFaceDetail1(appearenceValue);
                    break;
                case "faceDetail2":
                    SetFaceDetail2(appearenceValue);
                    break;
                case "hair":
                    SetHair(appearenceValue);
                    break;
                case "tops":
                    SetTop(appearenceValue);
                    break;
                case "bottoms":
                    SetBottom(appearenceValue);
                    break;
                case "shoes":
                    SetShoes(appearenceValue);
                    break;
                case "headAccessories":
                    SetHeadAccessory(appearenceValue);
                    break;
                case "backAccessories":
                    SetBackAccessory(appearenceValue);
                    break;
            }

            RegenerateFace();
        }

        public void SetAppearenceValue(string appearenceType, float appearenceValue)
        {
            switch(appearenceType)
            {
                case "faceDetail1Opacity":
                    SetFaceDetail1Opacity(appearenceValue);
                    break;
                case "faceDetail2Opacity":
                    SetFaceDetail2Opacity(appearenceValue);
                    break;
            }

            RegenerateFace();
        }

        public void SetAppearenceValue(string appearenceType, bool appearenceValue)
        {
            switch(appearenceType)
            {
                case "faceDetail1FlipHorizontal":
                    SetFaceDetail1FlipHorizontal(appearenceValue);
                    break;
                case "faceDetail2FlipHorizontal":
                    SetFaceDetail2FlipHorizontal(appearenceValue);
                    break;
            }

            RegenerateFace();
        }

        public void ToggleAppearenceValue(string appearenceType)
        {
            switch(appearenceType)
            {
                case "faceDetail1FlipHorizontal":
                    SetAppearenceValue(appearenceType, !appearence.faceDetail1FlipHorizontal);
                    break;
                case "faceDetail2FlipHorizontal":
                    SetAppearenceValue(appearenceType, !appearence.faceDetail2FlipHorizontal);
                    break;
            }
        }

        public void SetAppearenceValue(string appearenceType, Color appearenceValue)
        {
            switch(appearenceType)
            {
                case "eyeColours":
                    SetEyeColour(appearenceValue);
                    break;
                case "skinColours":
                    SetSkinColour(appearenceValue);
                    break;
                case "hairColours":
                    SetHairColour(appearenceValue);
                    break;
            }
        }

        public void GrabRotate(float amount)
        {
            rigidBody.AddTorque(transform.up * -amount);
        }

    }
}