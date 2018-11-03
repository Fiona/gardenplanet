using UnityEngine;

namespace StompyBlondie
{
    public static class ExtensionUnityVector3
    {
        public static Pos ToPos(this Vector3 input)
        {
            return new Pos {X = input.x, Y = input.y, Z = input.z};
        }

        public static Vector3 ToVector3(this Pos input)
        {
            return new Vector3(input.X, input.Y, input.Z);
        }
    }
}