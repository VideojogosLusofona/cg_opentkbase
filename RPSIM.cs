using OpenTK.Graphics.OpenGL;

namespace OpenTKBase
{
    public class RPSIM : RenderPipeline
    {
        public override void Render(Scene scene)
        {
            if (scene == null) return;

            var allCameras = scene.FindObjectsOfType<Camera>();
            var allRender = scene.FindObjectsOfType<Renderable>();

            foreach (var camera in allCameras)
            {
                // Clear color buffer and the depth buffer
                GL.ClearColor(camera.GetClearColor());
                GL.ClearDepth(camera.GetClearDepth());
                GL.Clear(camera.GetClearFlags());

                var cameraMatrix = camera.projection;
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadMatrix(ref cameraMatrix);

                foreach (var render in allRender)
                {
                    render.RenderImmediate(camera);
                }
            }
        }
    }
}
