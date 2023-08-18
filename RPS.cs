using System;
using OpenTK.Graphics.OpenGL;

namespace OpenTKBase
{
    public class RPS : RenderPipeline
    {
        public override void Render(Scene scene)
        {
            if (scene == null) return;

            var allCameras = scene.FindObjectsOfType<Camera>();
            var allRender = scene.FindObjectsOfType<Renderable>();
            var allLights = scene.FindObjectsOfType<Light>();

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
            }

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
                    render.Render(camera);
                }
            }
        }
    }
}
