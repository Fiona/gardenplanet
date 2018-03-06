using UnityEngine;

namespace StrawberryNova
{
    public class CACCharacter : Character
    {

        public override void Awake()
        {
            appearence = new Appearence
            {
                top = "ilovefarmingshirt",
                bottom = "",
                shoes = "",
                headAccessory = "",
                backAccessory = "",

                eyebrows = "thin",
                eyes = "cute",
                mouth = "kind_smile",
                nose = "small",

                eyeColor = Color.green,
                skinColor = Color.blue,
                hairColor = Color.red
            };
            information = new Information
            {
                Name = "",
                seasonBirthday = 1,
                dayBirthday = 1
            };
        }

        public override void Start()
        {
            RegenerateVisuals();
            RegenerateFace();
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
            }
        }

    }
}