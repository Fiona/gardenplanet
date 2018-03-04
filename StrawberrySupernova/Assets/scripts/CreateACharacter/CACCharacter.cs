using UnityEngine;

namespace StrawberryNova
{
    public class CACCharacter: Character
    {
        public override void Awake()
        {
        }

        public override void Start()
        {
            appearence = new Appearence
            {
                topName = "ilovefarmingshirt",
                bottomName = "",
                shoesName = "",
                headAccessoryName = "",
                backAccessoryName = "",

                eyesName = "",
                mouthName = "",
                noseName = "",

                eyeColor = Color.green,
                skinColor = Color.blue,
                hairColor = Color.red
            };
            RegenerateVisuals();
        }

        public override void FixedUpdate()
        {
        }

    }
}