using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;

namespace OpenTKBase
{
    public class MeshRenderer : Renderable
    {
        private List<Material>  _materials;

        public Material material
        {
            get
            {
                if (_materials == null) return null;
                if (_materials.Count == 0) return null;
                return _materials[0];
            }
            set
            {
                if (_materials == null) { _materials = new List<Material>(); _materials.Add(value); }
                else if (_materials.Count == 0) { _materials.Add(value); }
                else _materials[0] = value;
            }
        }

        public override void RenderImmediate(Camera camera)
        {
            var worldMatrix = transform.localToWorldMatrix;
            var cameraMatrix = camera.transform.worldToLocalMatrix;
            var matrix = worldMatrix * cameraMatrix;

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadMatrix(ref matrix);

            var material = this.material;
            material?.SetImmediate();

            var mf = GetComponent<MeshFilter>();
            mf?.mesh?.RenderImmediate();
        }
    }
}
