
using System;

namespace SDLBase
{
    // Basic implementation of a 2D vector
    public struct Vector2
    {
        public float x;
        public float y;

        public Vector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public static Vector2 operator +(Vector2 a, Vector2 b) => new Vector2(a.x + b.x, a.y + b.y);
        public static Vector2 operator +(Vector2 a, float b) => new Vector2(a.x + b, a.y + b);
        public static Vector2 operator -(Vector2 a, Vector2 b) => new Vector2(a.x - b.x, a.y - b.y);
        public static Vector2 operator -(Vector2 a) => new Vector2(-a.x, -a.y);
        public static Vector2 operator *(Vector2 v, float s) => new Vector2(v.x * s, v.y * s);
        public static Vector2 operator *(float s, Vector2 v) => new Vector2(v.x * s, v.y * s);
        public static Vector2 operator /(Vector2 v, float s) => new Vector2(v.x / s, v.y / s);
        public static bool operator ==(Vector2 a, Vector2 b) => a.Equals(b);
        public static bool operator !=(Vector2 a, Vector2 b) => !a.Equals(b);

        public override bool Equals(object obj)
        {
            Vector2 tmp = (Vector2)obj;
            return (tmp.x == x) && (tmp.y == y);
        }
        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }

        static public float Dot(Vector2 a, Vector2 b) => a.x * b.x + a.y * b.y;

        public float magnitude
        {
            get { return (float)Math.Sqrt(x * x + y * y); }
        }

        public Vector2 normalized
        {
            get { return this * (1.0f / magnitude); }
        }

        public override string ToString()
        {
            return $"({x:F3},{y:F3})";
        }

        public static Vector2 zero = new Vector2(0,0);
        public static Vector2 one = new Vector2(1,1);
        public static Vector2 up = new Vector2(0, 1);
        public static Vector2 right = new Vector2(1, 0);
        public static Vector2 down = new Vector2(0, -1);
        public static Vector2 left = new Vector2(-1, 0);
    }
}
