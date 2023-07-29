
namespace SDLBase
{
    public struct Color
    {
        public float r, g, b, a;

        public Color(float v)
        {
            r = v;
            g = v;
            b = v;
            a = 1;
        }

        public Color(float r, float g, float b, float a = 1.0f)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public void Set(float r, float g, float b, float a = 1.0f)
        {
            this.r = r;
            this.g = g;
            this.b = b;
            this.a = a;
        }

        public static explicit operator Color(Color32 v)
        {
            return new Color(v.r / 255.0f, v.g / 255.0f, v.b / 255.0f, v.a / 255.0f);
        }

        public static Color operator +(Color a, Color b) => new Color(a.r + b.r, a.g + b.g, a.b + b.b, a.a + b.a);
        public static Color operator -(Color a, Color b) => new Color(a.r - b.r, a.g - b.g, a.b - b.b, a.a - b.a);
        public static Color operator -(Color a, float v) => new Color(a.r - v, a.g - v, a.b - v, a.a - v);
        public static Color operator *(Color v, float s) => new Color(v.r * s, v.g * s, v.b * s, v.a * s);
        public static Color operator *(float s, Color v) => new Color(v.r * s, v.g * s, v.b * s, v.a * s);
        public static Color operator *(Color c1, Color c2) => new Color(c1.r * c2.r, c1.g * c2.g, c1.b * c2.b, c1.a * c2.a);
        public static Color operator /(Color v, float s) => new Color(v.r / s, v.g / s, v.b / s, v.a / s);

        public override string ToString()
        {
            return $"({r:F3},{g:F3},{b:F3},{a:F3})";
        }

        public static Color Lerp(Color c1, Color c2, float t)
        {
            return c1 + (c2 - c1) * t;
        }

        public static Color black = new Color(0, 0, 0, 1);
        public static Color white = new Color(1, 1, 1, 1);
        public static Color grey = new Color(0.5f, 0.5f, 0.5f, 1);
        public static Color red = new Color(1, 0, 0, 1);
        public static Color green = new Color(0, 1, 0, 1);
        public static Color blue = new Color(0, 0, 1, 1);
        public static Color cyan = new Color(0, 1, 1, 1);
        public static Color magenta = new Color(1, 0, 1, 1);
        public static Color yellow = new Color(1, 1, 0, 1);
        public static Color unityBlue = new Color(0.34f, 0.42f, 0.56f, 1.0f);
    }
}
