using System;
using UnityEngine;

namespace GardenPlanet
{
    public class CACCharacter : Character
    {
        public override void Start()
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
                case "face_detail1":
                    SetFaceDetail1(appearenceValue);
                    break;
                case "face_detail2":
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
                case "head_accessories":
                    SetHeadAccessory(appearenceValue);
                    break;
                case "back_accessories":
                    SetBackAccessory(appearenceValue);
                    break;
            }

            RegenerateFace();
        }

        public void SetAppearenceValue(string appearenceType, float appearenceValue)
        {
            switch(appearenceType)
            {
                case "face_detail1_opacity":
                    SetFaceDetail1Opacity(appearenceValue);
                    break;
                case "face_detail2_opacity":
                    SetFaceDetail2Opacity(appearenceValue);
                    break;
            }

            RegenerateFace();
        }

        public void SetAppearenceValue(string appearenceType, bool appearenceValue)
        {
            switch(appearenceType)
            {
                case "face_detail1_flip_horizontal":
                    SetFaceDetail1FlipHorizontal(appearenceValue);
                    break;
                case "face_detail2_flip_horizontal":
                    SetFaceDetail2FlipHorizontal(appearenceValue);
                    break;
            }

            RegenerateFace();
        }

        public void ToggleAppearenceValue(string appearenceType)
        {
            switch(appearenceType)
            {
                case "face_detail1_flip_horizontal":
                    SetAppearenceValue(appearenceType, !appearence.faceDetail1FlipHorizontal);
                    break;
                case "face_detail2_flip_horizontal":
                    SetAppearenceValue(appearenceType, !appearence.faceDetail2FlipHorizontal);
                    break;
            }
        }

        public void SetAppearenceValue(string appearenceType, Color appearenceValue)
        {
            switch(appearenceType)
            {
                case "eye_colours":
                    SetEyeColour(appearenceValue);
                    break;
                case "skin_colours":
                    SetSkinColour(appearenceValue);
                    break;
                case "hair_colours":
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