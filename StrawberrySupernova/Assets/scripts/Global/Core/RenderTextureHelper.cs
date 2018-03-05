using UnityEngine;

namespace StompyBlondie
{
    public static class RenderTextureHelper
    {
        public static void Clear(this RenderTexture texture, Color color)
        {
            var rt = RenderTexture.active;
            RenderTexture.active = texture;
            GL.Clear(true, true, color);
            RenderTexture.active = rt;
        }
    }
}