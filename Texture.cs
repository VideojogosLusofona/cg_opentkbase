

using System;
using System.IO;
using OpenTK.Compute.OpenCL;
using OpenTK.Graphics.OpenGL;
using StbImageSharp;

namespace OpenTKBase
{
    public class Texture
    {
        private int         handle = -1;
        TextureWrapMode     wrapMode;
        TextureMinFilter    filterMode;
        bool                enableMips;

        public Texture(TextureWrapMode wrapMode = TextureWrapMode.Repeat, TextureMinFilter filterMode = TextureMinFilter.Linear, bool enableMips = true)
        {
            this.wrapMode = wrapMode;
            this.filterMode = filterMode;
            this.enableMips = enableMips;
        }

        ~Texture()
        {
            if (handle != -1)
            {
                GL.DeleteTexture(handle);
                handle = -1;
            }
        }

        public bool Load(string filename)
        {
            // stb_image loads from the top-left pixel, whereas OpenGL loads from the bottom-left, causing the texture to be flipped vertically.
            // This will correct that, making the texture display properly.
            StbImage.stbi_set_flip_vertically_on_load(1);

            // Load the image.
            ImageResult image = ImageResult.FromStream(File.OpenRead(filename), ColorComponents.RedGreenBlueAlpha);
            if (image == null)
            {
                Console.WriteLine($"Error reading {filename}!");
                return false;
            }

            // Create image
            handle = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, handle);

            // Load image into texture
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, image.Width, image.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)wrapMode);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)wrapMode);

            if (enableMips)
            {
                GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
                switch (filterMode)
                {
                    case TextureMinFilter.Nearest:
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.NearestMipmapNearest);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                        break;
                    case TextureMinFilter.Linear:
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.LinearMipmapLinear);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                        break;
                }
            }
            else
            {
                switch (filterMode)
                {
                    case TextureMinFilter.Nearest:
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Nearest);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Nearest);
                        break;
                    case TextureMinFilter.Linear:
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                        break;
                }
            }

            // Unset texture (keep things clean)
            GL.BindTexture(TextureTarget.Texture2D, 0);

            return true;
        }


        public void Set(int textureUnit)
        {
            GL.ActiveTexture(TextureUnit.Texture0 + textureUnit);
            GL.BindTexture(TextureTarget.Texture2D, handle);
        }
    }
}
