using System;
using OpenTK.Mathematics;

namespace OpenTKBase
{
    public class Light : Component
    {
        public enum Type { Directional = 0, Point = 1, Spot = 2};

        public Type     type = Type.Directional;
        public Color4   lightColor = new Color4(255, 255, 255, 255);
        public float    intensity = 1.0f;
        public Vector2  cone = new Vector2(MathF.PI * 0.25f, MathF.PI * 0.5f);
        public float    range = 5.0f;

        private bool    shadowmapEnable = false;
        private Texture shadowmap;
        private int     shadowmapResolution = 2048;

        
        public void SetShadow(bool enable, int resolution)
        {
            shadowmapEnable = enable;
            shadowmapResolution = resolution;
            if (enable)
            {
                shadowmap = new Texture(OpenTK.Graphics.OpenGL.TextureWrapMode.ClampToBorder, OpenTK.Graphics.OpenGL.TextureMinFilter.Nearest, false);
                shadowmap.CreateDepth(shadowmapResolution, shadowmapResolution);
                shadowmap.CreateRendertarget();
            }
            else
            {
                shadowmap = null;
            }
        }

        public bool HasShadowmap() => shadowmapEnable;
        public Texture GetShadowmap() => shadowmap;

        public Matrix4 GetSpotlightProjection()
        {
            return Matrix4.CreatePerspectiveFieldOfView(cone.Y, 1.0f, 0.01f, range);
        }

        public Matrix4 GetShadowMatrix()
        {
            return transform.worldToLocalMatrix * GetSpotlightProjection();
        }
    }
}
