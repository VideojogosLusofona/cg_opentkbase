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
            // Have found at least a file with the name "{name}.*"
            bool fileFound = false;

            // Handles of the loaded shaders
            int[] shaderHandle = new int[TypeMax] { -1, -1 };

            // For each shader type
            for (int i = 0; i < TypeMax; i++)
            {
                // Check if a file exist with the name "{name}.{extension}"
                string filename = $"{name}.{GetExtension(i)}";
                if (File.Exists(filename))
                {
                    // Found a file
                    fileFound |= true;

                    // Load the file
                    string source = File.ReadAllText(filename);

                    // Create a shader of this type
                    shaderHandle[i] = GL.CreateShader(GLShaderTypeFromType(i));

                    // Set the shader source to the loaded data
                    GL.ShaderSource(shaderHandle[i], source);
                    // Compile the shader
                    GL.CompileShader(shaderHandle[i]);
                    // Check for compilation errors
                    GL.GetShader(shaderHandle[i], ShaderParameter.CompileStatus, out int errorCode);
                    if (errorCode == 0)
                    {
                        // If there are errors, log them
                        string infoLog = GL.GetShaderInfoLog(handle);
                        Console.WriteLine(infoLog);

                        // Clear all shaders before this one
                        for (int j = 0; j < i; j++)
                        {
                            if (shaderHandle[j] != -1) GL.DeleteShader(shaderHandle[j]);
                        }

                        // Return false, process has failed
                        return false;
                    }
                }
            }

            // At least one file was loaded
            if (fileFound)
            {
                // Create a program
                handle = GL.CreateProgram();

                // Attach all shaders to this program
                for (int i = 0; i < TypeMax; i++)
                {
                    if (shaderHandle[i] != -1)
                    {
                        GL.AttachShader(handle, shaderHandle[i]);
                    }
                }

                // Link the program
                GL.LinkProgram(handle);

                // Cleanup (no need for shaders to be attached to program anymore, and they can be deleted)
                for (int i = 0; i < TypeMax; i++)
                {
                    if (shaderHandle[i] != -1)
                    {
                        GL.DetachShader(handle, shaderHandle[i]);
                        GL.DeleteShader(shaderHandle[i]);
                    }
                }

                // Check if there were linking errors
                GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out int errorCode);
                if (errorCode == 0)
                {
                    // If there's an error, log it
                    string infoLog = GL.GetProgramInfoLog(handle);
                    Console.WriteLine(infoLog);

                    // Delete the program
                    GL.DeleteShader(handle);

                    // Return false, process has failed
                    return false;
                }
            }
            else
            {
                // No file was found with the given name
                Console.WriteLine($"No file {name}.* found (current directory = {System.IO.Directory.GetCurrentDirectory()})!");
                return false;
            }

            // Process has completed successfully
            return true;
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
