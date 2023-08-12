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
    }
}
