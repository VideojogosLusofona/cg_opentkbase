using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKBase
{
    public class Shader
    {
        public const int Vertex = 0;
        public const int Fragment = 1;
        public const int TypeMax = 2;

        private int handle = -1;

        ~Shader()
        {
            if (handle != -1)
            {
                GL.DeleteProgram(handle);
                handle = -1;
            }
        }

        public static string GetExtension(int type)
        {
            switch (type)
            {
                case Vertex: return "vert";
                case Fragment: return "frag";
            }
            return "";
        }

        private static ShaderType GLShaderTypeFromType(int type)
        {
            switch (type)
            {
                case Vertex: return ShaderType.VertexShader;
                case Fragment: return ShaderType.FragmentShader;
            }
            return ShaderType.ComputeShader;
        }

        public bool Load(string name)
        {
            bool success = false;
            bool fileFound = false;

            int[] shaderHandle = new int[TypeMax] { -1, -1 };

            for (int i = 0; i < TypeMax; i++)
            {
                string filename = $"{name}.{GetExtension(i)}";
                if (File.Exists(filename))
                {
                    fileFound |= true;

                    string source = File.ReadAllText(filename);

                    shaderHandle[i] = GL.CreateShader(GLShaderTypeFromType(i));

                    GL.ShaderSource(shaderHandle[i], source);
                    GL.CompileShader(shaderHandle[i]);
                    GL.GetShader(shaderHandle[i], ShaderParameter.CompileStatus, out int errorCode);
                    if (errorCode == 0)
                    {
                        string infoLog = GL.GetShaderInfoLog(handle);
                        Console.WriteLine(infoLog);

                        // Clear all shaders before this one
                        for (int j = 0; j < i; j++)
                        {
                            if (shaderHandle[j] != -1) GL.DeleteShader(shaderHandle[j]);
                        }
                        return false;
                    }

                    success |= true;
                }
            }

            if (success)
            {
                // Link program
                handle = GL.CreateProgram();

                for (int i = 0; i < TypeMax; i++)
                {
                    if (shaderHandle[i] != -1)
                    {
                        GL.AttachShader(handle, shaderHandle[i]);
                    }
                }

                GL.LinkProgram(handle);

                GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int errorCode);
                if (errorCode == 0)
                {
                    string infoLog = GL.GetProgramInfoLog(handle);
                    Console.WriteLine(infoLog);
                    return false;
                }

                // Cleanup (no need for shaders to be attached to program anymore, and they can be deleted)
                for (int i = 0; i < TypeMax; i++)
                {
                    if (shaderHandle[i] != -1)
                    {
                        GL.DetachShader(handle, shaderHandle[i]);
                        GL.DeleteShader(shaderHandle[i]);
                    }
                }
            }
            else
            {
                if (!fileFound)
                {
                    Console.WriteLine($"No file {name}.* found (current directory = {System.IO.Directory.GetCurrentDirectory()})!");
                }
            }

            return success;
        }

        public int GetAttributePos(string attributeName)
        {
            return GL.GetAttribLocation(handle, attributeName);
        }

        public void Set()
        {
            if (handle != -1)
            {
                GL.UseProgram(handle);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // STATIC STUFF
        private static Dictionary<string, Shader> shaders = new Dictionary<string, Shader>();

        public static Shader Find(string name)
        {
            Shader shader = null;

            // See if we've already loaded this
            if (shaders.TryGetValue(name, out shader))
            {
                return shader;
            }

            // Find all shaders
            shader = new Shader();
            if (shader.Load(name))
            {
                shaders.Add(name, shader);

                return shader;
            }
            else
            {
                return null;
            }
        }
    }
}
