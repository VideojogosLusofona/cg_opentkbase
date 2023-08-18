using System;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace OpenTKBase
{
    public class RPS : RenderPipeline
    {
        static private Material shadowmapMaterial;
        static private Texture  defaultShadowmap;

        private Material GetShadowmapMaterial()
        {
            if (shadowmapMaterial == null)
            {
                shadowmapMaterial = new Material(Shader.Find("shaders/std_shadowmap"));
            }

            return shadowmapMaterial;
        }

        private Texture GetDefaultShadowmap()
        {
            if (defaultShadowmap == null)
            {
                defaultShadowmap = new Texture();
                defaultShadowmap.CreateDepth(1, 1);
            }

            return defaultShadowmap;
        }

        public override void Render(Scene scene)
        {
            if (scene == null) return;

            var allCameras = scene.FindObjectsOfType<Camera>();
            var allRender = scene.FindObjectsOfType<Renderable>();
            var allLights = scene.FindObjectsOfType<Light>();

            // Invert cull mode for shadowmap rendering (only works if objects are all "solid")
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Front);

            // Render shadowmaps, if needed
            foreach (var light in allLights)
            {
                if (light.HasShadowmap())
                {
                    // Get shadowmap material
                    var shadowmapMaterial = GetShadowmapMaterial();

                    if (light.type == Light.Type.Spot)
                    {
                        // Setup rendertarget
                        var shadowmap = light.GetShadowmap();
                        shadowmap.Set(-1);

                        GL.ClearDepth(1.0f);
                        GL.Clear(ClearBufferMask.DepthBufferBit);

                        Shader.SetMatrix(Shader.MatrixType.Camera, light.transform.worldToLocalMatrix);
                        Shader.SetMatrix(Shader.MatrixType.InvCamera, light.transform.localToWorldMatrix);
                        Shader.SetMatrix(Shader.MatrixType.Projection, light.GetSpotlightProjection());

                        foreach (var render in allRender)
                        {
                            render.Render(null, shadowmapMaterial);
                        }

                        shadowmap.Unset();
                    }
                    else
                    {
                        Console.WriteLine($"Unsupported light type {light.type} for shadowmap!");
                    }
                }
            }

            // Restore cull mode to normal
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);

            var envMaterial = OpenTKApp.APP.mainScene.environment;
            envMaterial.Set("LightCount", allLights.Count);
            for (int i = 0; i < Math.Min(allLights.Count, 8); i++)
            {
                var light = allLights[i];
                envMaterial.Set($"Lights[{i}].type", (int)light.type);
                envMaterial.Set($"Lights[{i}].position", light.transform.position);
                envMaterial.Set($"Lights[{i}].direction", light.transform.forward);
                envMaterial.Set($"Lights[{i}].color", light.lightColor);
                envMaterial.Set($"Lights[{i}].intensity", light.intensity);
                envMaterial.Set($"Lights[{i}].spot", light.cone.X * 0.5f, light.cone.Y * 0.5f, MathF.Cos(light.cone.X * 0.5f), MathF.Cos(light.cone.Y * 0.5f));
                envMaterial.Set($"Lights[{i}].range", light.range);
                envMaterial.Set($"Lights[{i}].shadowmapEnable", light.HasShadowmap());
                if (light.HasShadowmap())
                {
                    envMaterial.Set($"Lights[{i}].shadowmap", light.GetShadowmap().GetDepthTexture());
                }
                else
                {
                    envMaterial.Set($"Lights[{i}].shadowmap", GetDefaultShadowmap().GetDepthTexture());
                }
                envMaterial.Set($"Lights[{i}].shadowMatrix", light.GetShadowMatrix());
            }
            for (int i = Math.Min(allLights.Count, 8); i < 8; i++)
            {
                envMaterial.Set($"Lights[{i}].shadowmap", GetDefaultShadowmap().GetDepthTexture());
            }

            GL.Viewport(0, 0, OpenTKApp.APP.resX, OpenTKApp.APP.resY);

            foreach (var camera in allCameras)
            {
                // Clear color buffer and the depth buffer
                GL.ClearColor(camera.GetClearColor());
                GL.ClearDepth(camera.GetClearDepth());
                GL.Clear(camera.GetClearFlags());

                Shader.SetMatrix(Shader.MatrixType.Camera, camera.transform.worldToLocalMatrix);
                Shader.SetMatrix(Shader.MatrixType.InvCamera, camera.transform.localToWorldMatrix);
                Shader.SetMatrix(Shader.MatrixType.Projection, camera.projection);

                foreach (var render in allRender)
                {
                    render.Render(camera, null);
                }
            }
        }
    }
}
