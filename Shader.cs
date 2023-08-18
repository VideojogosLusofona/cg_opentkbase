using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
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
        public enum MatrixType { Identity = 0, Clip = 1, Projection = 2, Camera = 3, InvCamera = 4, World = 5, Max = 6 };

        static public MatrixType StringToMatrixType(string m)
        {
            switch (m)
            {
                case "Clip": return MatrixType.Clip;
                case "Projection": return MatrixType.Projection;
                case "Camera": return MatrixType.Camera;
                case "World": return MatrixType.World;
            }

            return MatrixType.Identity;
        }

        private struct Uniform
        {
            public enum Type { 
                                Material, Environment, Texture, EnvTexture, 
                                Matrix, 
                                ViewPos, ViewDir,
                                HasTexture
            };

            public Type                 type;
            public string               name;
            public MatrixType           matrixType;
            public ActiveUniformType    dataType;
            public int                  dataSize;
            public int                  slot;
        }

        private class MatrixData
        {
            public MatrixData(MatrixType type) { this.type = type; }

            private MatrixType  type;
            private bool        dirty;
            private Matrix4     matrix;

            public Matrix4 Get()
            {
                if (!dirty) return matrix;

                switch (type)
                {
                    case MatrixType.Clip:
                        matrix = currentMatrices[(int)MatrixType.World].matrix * currentMatrices[(int)MatrixType.Camera].matrix * currentMatrices[(int)MatrixType.Projection].matrix;
                        break;
                }

                dirty = false;
                return matrix;
            }

            public void Set(Matrix4 matrix)
            {
                this.matrix = matrix;

                switch (type)
                {
                    case MatrixType.Projection:
                    case MatrixType.Camera:
                    case MatrixType.InvCamera:
                    case MatrixType.World:
                        currentMatrices[(int)(MatrixType.Clip)].dirty = true;
                        break;
                    default:
                        break;
                }
            }
        };

        static private MatrixData[] currentMatrices;

        static public void SetMatrix(MatrixType type, Matrix4 matrix)
        {
            if (currentMatrices == null)
            {
                currentMatrices = new MatrixData[(int)MatrixType.Max];
                for (int i = 0; i < (int)MatrixType.Max; i++)
                {
                    currentMatrices[i] = new MatrixData((MatrixType)i);
                }
            }
            currentMatrices[(int)type].Set(matrix);
        }

        public const int Vertex = 0;
        public const int Fragment = 1;
        public const int TypeMax = 2;

        private string          name;
        private int             handle = -1;
        private List<Uniform>   uniforms;

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
            this.name = name;

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

            ParseUniforms();

            // Process has completed successfully
            return true;
        }

        public int GetAttributePos(string attributeName)
        {
            return GL.GetAttribLocation(handle, attributeName);
        }

        static int textureUnit = 0;

        public void Set(Material material)
        {
            if (handle != -1)
            {
                Matrix4 viewMatrix;

                GL.UseProgram(handle);

                textureUnit = 0;

                // Process uniforms
                foreach (var u in uniforms)
                {
                    switch (u.type)
                    {
                        case Uniform.Type.Material:
                            if (material != null) SetUniformMaterial(u, material);
                            break;
                        case Uniform.Type.Texture:
                            if (material != null)
                            {
                                material.GetProperty(u.name, out Texture texture);
                                if (texture != null)
                                {
                                    texture.Set(textureUnit);
                                    GL.Uniform1(u.slot, textureUnit++);
                                }
                            }
                            break;
                        case Uniform.Type.EnvTexture:
                            if (OpenTKApp.APP.mainScene.environment != null)
                            {
                                OpenTKApp.APP.mainScene.environment.GetProperty(u.name, out Texture texture);
                                if (texture != null)
                                {
                                    texture.Set(textureUnit);
                                    GL.Uniform1(u.slot, textureUnit++);
                                }
                            }
                            break;
                        case Uniform.Type.HasTexture:
                            if (material == null) GL.Uniform1(u.slot, 0);
                            else GL.Uniform1(u.slot, material.HasProperty(u.name)?(1):(0));
                            break;
                        case Uniform.Type.Matrix:
                            SetUniformMatrix(u, material);
                            break;
                        case Uniform.Type.Environment:
                            if (OpenTKApp.APP.mainScene.environment != null) SetUniformMaterial(u, OpenTKApp.APP.mainScene.environment);
                            break;
                        case Uniform.Type.ViewPos:
                            viewMatrix = currentMatrices[(int)MatrixType.InvCamera].Get();
                            var viewPos = (Vector4.UnitW * viewMatrix).Xyz;
                            GL.Uniform3(u.slot, viewPos);
                            break;
                        case Uniform.Type.ViewDir:
                            viewMatrix = currentMatrices[(int)MatrixType.InvCamera].Get();
                            var viewDir = (Vector4.UnitZ * viewMatrix).Xyz;
                            GL.Uniform3(u.slot, viewDir);
                            break;
                        default:
                            Console.Write($"Unhandled type of constant {u.type}!\n");
                            break;
                    }
                }
            }
        }

        static private void SetUniformMaterial(Uniform u, Material material)
        {
            // Check if property exists in material
            if (material.GetProperty(u.name, out object value))
            {
                switch (u.dataType)
                {
                    case ActiveUniformType.Bool:
                        GL.Uniform1(u.slot, ((bool)value)?(1):(0));
                        break;
                    case ActiveUniformType.Int:
                        GL.Uniform1(u.slot, (int)value);
                        break;
                    case ActiveUniformType.Float:
                        GL.Uniform1(u.slot, (float)value);
                        break;
                    case ActiveUniformType.FloatVec2:
                        GL.Uniform2(u.slot, (Vector2)value);
                        break;
                    case ActiveUniformType.FloatVec3:
                        GL.Uniform3(u.slot, (Vector3)value);
                        break;
                    case ActiveUniformType.FloatVec4:
                        GL.Uniform4(u.slot, (Vector4)value);
                        break;
                    case ActiveUniformType.FloatMat4:
                        var matrix4 = (Matrix4)value;
                        GL.UniformMatrix4(u.slot, false, ref matrix4);
                        break;
                    case ActiveUniformType.Sampler2D:
                    case ActiveUniformType.Sampler2DShadow:
                        var texture = (Texture)value;
                        if (texture != null)
                        {
                            texture.Set(textureUnit);
                            GL.Uniform1(u.slot, textureUnit++);
                        }
                        else
                        {
                            GL.Uniform1(u.slot, 0);
                        }
                        break;
                    default:
                        Console.WriteLine($"Unsupported uniform data type {u.dataType} (type={u.type}, name={u.name})!");
                        break;
                }
            }
            else
            {
                // Material is allowed not to have a texture (shader needs to account for that), so no warning is thrown if it's a variable of this type
                if (u.dataType == ActiveUniformType.Sampler2D) return;

                Console.WriteLine($"Material doesn't have property {u.name}!");
            }
        }

        static private void SetUniformMatrix(Uniform u, Material material)
        {
            switch (u.dataType)
            {
                case ActiveUniformType.FloatMat4:
                    // See if matrix needs to be refresh
                    var matrix = currentMatrices[(int)u.matrixType].Get();
                    GL.UniformMatrix4(u.slot, false, ref matrix);
                    break;
                default:
                    Console.WriteLine($"Unsupported matrix uniform data type {u.dataType} (type={u.matrixType}, name={u.name})!");
                    break;
            }
        }

        public void ParseUniforms()
        {
            uniforms = new List<Uniform>();

            // Get number of uniforms
            GL.GetProgram(handle, GetProgramParameterName.ActiveUniforms, out int numUniforms);
            for (int i = 0; i < numUniforms; i++)
            {
                GL.GetActiveUniform(handle, i, 256, out int length, out int size, out ActiveUniformType type, out string uniformName);
                int location = GL.GetUniformLocation(handle, uniformName);

                if (uniformName.StartsWith("Material"))
                {
                    // This is a material property
                    uniforms.Add(new Uniform()
                    {
                        type = Uniform.Type.Material,
                        name = uniformName.Substring(8),
                        slot = location,
                        dataSize = size,
                        dataType = type
                    });
                }
                else if (uniformName.StartsWith("Texture"))
                {
                    // This is a material property
                    uniforms.Add(new Uniform()
                    {
                        type = Uniform.Type.Texture,
                        name = uniformName.Substring(7),
                        slot = location,
                        dataSize = size,
                        dataType = type
                    });
                }
                else if (uniformName.StartsWith("EnvTexture"))
                {
                    // This is a material property
                    uniforms.Add(new Uniform()
                    {
                        type = Uniform.Type.EnvTexture,
                        name = uniformName.Substring(10),
                        slot = location,
                        dataSize = size,
                        dataType = type
                    });
                }
                else if (uniformName.StartsWith("Matrix"))
                {
                    // This is a material property
                    uniforms.Add(new Uniform()
                    {
                        type = Uniform.Type.Matrix,
                        name = uniformName.Substring(6),
                        matrixType = StringToMatrixType(uniformName.Substring(6)),
                        slot = location,
                        dataSize = size,
                        dataType = type
                    });
                }
                else if (uniformName.StartsWith("Env"))
                {
                    // This is a material property
                    uniforms.Add(new Uniform()
                    {
                        type = Uniform.Type.Environment,
                        name = uniformName.Substring(3),
                        slot = location,
                        dataSize = size,
                        dataType = type
                    });
                }
                else if (uniformName.StartsWith("Light"))
                {
                    uniforms.Add(new Uniform()
                    {
                        type = Uniform.Type.Environment,
                        name = uniformName,
                        slot = location,
                        dataSize = size,
                        dataType = type
                    });
                }
                else if (uniformName.StartsWith("HasTexture"))
                {
                    // This is a material property
                    uniforms.Add(new Uniform()
                    {
                        type = Uniform.Type.HasTexture,
                        name = uniformName.Substring(10),
                        slot = location,
                        dataSize = size,
                        dataType = type
                    });
                }
                else if (uniformName == "ViewPos") uniforms.Add(new Uniform() { type = Uniform.Type.ViewPos, name = uniformName, slot = location, dataSize = size, dataType = type });
                else if (uniformName == "ViewDir") uniforms.Add(new Uniform() { type = Uniform.Type.ViewDir, name = uniformName, slot = location, dataSize = size, dataType = type });
                else
                {
                    Console.WriteLine($"Can't parse uniform {uniformName} in shader {name}!");
                }
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
