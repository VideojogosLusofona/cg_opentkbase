

using System;
using System.IO;
using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace OpenTKBase
{
    public class Texture
    {
        private int                 handle = -1;
        private TextureWrapMode     wrapMode;
        private TextureMinFilter    filterMode;
        private bool                enableMips;
        private bool                isCube = false;
        private bool                isRendertarget = false;
        private Texture             colorTexture;
        private Texture             depthTexture;
        private int                 width, height;

        public Texture(TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureMinFilter filterMode = TextureMinFilter.Linear, bool enableMips = true)
        {
            this.wrapMode = wrapMode;
            this.filterMode = filterMode;
            this.enableMips = enableMips;
            width = height = 0;
        }

        ~Texture()
        {
            if (handle != -1)
            {
                GL.DeleteTexture(handle);
                handle = -1;
            }
        }

        public bool LoadCube(string base_filename)
        {
            // Create image
            handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.TextureCubeMap, handle);

            string[] postfix = new string[6] { "posx", "negx", "posy", "negy", "posz", "negz" };

            for (int i = 0; i < 6; i++)
            {
                string filename = base_filename.Replace("*", postfix[i]);

                // Load the image.
                ImageResult image = ImageResult.FromStream(File.OpenRead(filename), ColorComponents.RedGreenBlueAlpha);
                if (image == null)
                {
                    Console.WriteLine($"Error reading {filename}!");
                    return false;
                }

                // Load image into texture
                GL.TexImage2D(TextureTarget.TextureCubeMapPositiveX + i, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);

                width = Math.Max(image.Width, width);
                height = Math.Max(image.Height, height);
            }

            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR, (int)TextureWrapMode.ClampToEdge);

            if (enableMips)
            {
                GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
                switch (filterMode)
                {
                    case TextureMinFilter.Nearest:
                        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
                        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                        break;
                    case TextureMinFilter.Linear:
                        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                        break;
                }
            }
            else
            {
                switch (filterMode)
                {
                    case TextureMinFilter.Nearest:
                        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                        break;
                    case TextureMinFilter.Linear:
                        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                        GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                        break;
                }
            }

            // Unset texture (keep things clean)
            GL.BindTexture(TextureTarget.TextureCubeMap, 0);

            isCube = true;

            return true;
        }

        public bool Load(string filename)
        {
            // stb_image loads from the top-left pixel, whereas OpenGL loads from the bottom-left, causing the texture to be flipped vertically.
            // This will correct that, making the texture display properly.
            // StbImage.stbi_set_flip_vertically_on_load(1);

            // Load the image.
            ImageResult image = ImageResult.FromStream(File.OpenRead(filename), ColorComponents.RedGreenBlueAlpha);
            if (image == null)
            {
                Console.WriteLine($"Error reading {filename}!");
                return false;
            }

            Create(TextureTarget.Texture2D, PixelInternalFormat.Rgba, PixelFormat.Rgba, PixelType.UnsignedByte, image.Width, image.Height, image.Data);

            width = image.Width;
            height = image.Height;

            return true;
        }

        public bool CreateDepth(int width = 0, int height = 0)
        {
            if (width != 0) this.width = width;
            if (height != 0) this.height = height;

            // Setup wrap mode of depth texture to be clamp to border...
            depthTexture = new Texture(TextureWrapMode.ClampToBorder, TextureMinFilter.Linear, false);
            depthTexture.Create(TextureTarget.Texture2D, PixelInternalFormat.DepthComponent, PixelFormat.DepthComponent, PixelType.Float, this.width, this.height, null);

            // Sampling outside should return 1.0f
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureBorderColor, new float[4] { 1.0f, 1.0f, 1.0f, 1.0f });
            // Need to setup the comparison operator as well
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareMode, (int)All.CompareRefToTexture);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureCompareFunc, (int)All.Lequal);

            return true;
        }

        public Texture GetDepthTexture() => depthTexture;

        public void CreateRendertarget()
        {
            isRendertarget = true;

            handle = GL.GenFramebuffer();

            GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);
            if (colorTexture != null)
            {
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, colorTexture.handle, 0);
            }
            else
            {
                GL.DrawBuffer(DrawBufferMode.None);
                GL.ReadBuffer(ReadBufferMode.None);
            }
            if (depthTexture != null)
            {
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.DepthAttachment, TextureTarget.Texture2D, depthTexture.handle, 0);
            }
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        private void Create(TextureTarget targetType, PixelInternalFormat format, PixelFormat pixelFormat, PixelType pixelType, int width, int height, byte[] data)
        {
            // Create image
            handle = GL.GenTexture();
            GL.BindTexture(targetType, handle);

            // Load image into texture
            GL.TexImage2D(targetType, 0, format, width, height, 0, pixelFormat, pixelType, data);
            GL.TexParameter(targetType, TextureParameterName.TextureWrapS, (int)wrapMode);
            GL.TexParameter(targetType, TextureParameterName.TextureWrapT, (int)wrapMode);

            if (enableMips)
            {
                switch (targetType)
                {
                    case TextureTarget.Texture2D:
                        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                        break;
                    case TextureTarget.TextureCubeMap:
                        GL.GenerateMipmap(GenerateMipmapTarget.TextureCubeMap);
                        break;
                    default:
                        Console.WriteLine($"Failed to generate mip-maps, unknown target type {targetType}!");
                        break;
                }
                switch (filterMode)
                {
                    case TextureMinFilter.Nearest:
                        GL.TexParameter(targetType, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
                        GL.TexParameter(targetType, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                        break;
                    case TextureMinFilter.Linear:
                        GL.TexParameter(targetType, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                        GL.TexParameter(targetType, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                        break;
                }
            }
            else
            {
                switch (filterMode)
                {
                    case TextureMinFilter.Nearest:
                        GL.TexParameter(targetType, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                        GL.TexParameter(targetType, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                        break;
                    case TextureMinFilter.Linear:
                        GL.TexParameter(targetType, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                        GL.TexParameter(targetType, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                        break;
                }
            }
        }

        public void Set(int textureUnit)
        {
            if (isRendertarget)
            {
                // Set as a rendertarget
                GL.Viewport(0, 0, width, height);
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, handle);
            }
            else
            {
                // Set as an input texture to a shader
                GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
                if (isCube)
                {
                    GL.BindTexture(TextureTarget.TextureCubeMap, handle);
                }
                else
                {
                    GL.BindTexture(TextureTarget.Texture2D, handle);
                }
            }
        }

        public void Unset()
        {
            if (isRendertarget)
            {
                GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
            }
        }
    }
}
