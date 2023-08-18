
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Drawing;

namespace OpenTKBase
{
    public class Material
    {
        public Shader                       shader;
        public Dictionary<string, object>   properties;

        public Material(Shader shader)
        {
            this.shader = shader;
            properties = new Dictionary<string, object>();
        }

        public void Set(string name, object value)
        {
            properties[name] = value;
        }
        public void Set(string name, double value)
        {
            properties[name] = (float)value;
        }

        public void Set(string name, int value)
        {
            properties[name] = (int)value;
        }
        
        public void Set(string name, Color4 value)
        {
            properties[name] = new Vector4(value.R, value.G, value.B, value.A);
        }
        public void Set(string name, float x, float y, float z, float w)
        {
            properties[name] = new Vector4(x, y, z, w);
        }

        public T Get<T>(string name) => (T)properties[name];

        public bool GetProperty(string name, out object value)
        {
            return properties.TryGetValue(name, out value);
        }
        public bool GetProperty<T>(string name, out T value) where T : class
        {
            var b = properties.TryGetValue(name, out object val);
            if (b)
            {
                value = val as T;
            }
            else value = null;
            return b;
        }

        public bool HasProperty(string name)
        {
            return properties.ContainsKey(name);
        }
    }
}
