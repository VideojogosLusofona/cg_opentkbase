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
            envMaterial.SetInt("LightCount", allLights.Count);
            for (int i = 0; i < Math.Min(allLights.Count, 8); i++)
            {
                var light = allLights[i];
                envMaterial.SetInt($"Lights[{i}].type", (int)light.type);
                envMaterial.SetVector3($"Lights[{i}].position", light.transform.position);
                envMaterial.SetVector3($"Lights[{i}].direction", light.transform.forward);
                envMaterial.SetColor($"Lights[{i}].color", light.lightColor);
                envMaterial.SetFloat($"Lights[{i}].intensity", light.intensity);
                envMaterial.SetVector2($"Lights[{i}].spot", light.cone); 
            }

            foreach (var camera in allCameras)
            {
                // Clear color buffer and the depth buffer
                GL.ClearColor(camera.GetClearColor());
                GL.ClearDepth(camera.GetClearDepth());
                GL.Clear(camera.GetClearFlags());

                Shader.SetMatrix(Shader.MatrixType.Camera, camera.transform.worldToLocalMatrix);
                Shader.SetMatrix(Shader.MatrixType.Projection, camera.projection);

                foreach (var render in allRender)
                {
                    render.Render(camera);
                }
            }
        }
    }
}
