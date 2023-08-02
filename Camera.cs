using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKBase
{
    public class Camera : Component
    {
        private ClearBufferMask _clearFlags = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit;
        private Color4          _clearColor = Color4.Teal;
        private float           _clearDepth = 1.0f;
        private bool            _ortographic = true;
        private float           _ortographicSize = 360.0f;
        private float           _fieldOfView = MathF.PI / 3.0f;
        private float           _nearPlane = 0.03f;
        private float           _farPlane = 1000.0f;
        private bool            _dirty = true;
        private Matrix4         _projection;

        public ClearBufferMask  GetClearFlags() => _clearFlags;
        public Color4           GetClearColor() => _clearColor;
        public float            GetClearDepth() => _clearDepth;
        
        public bool             ortographic { get => _ortographic; set { _ortographic = value; _dirty = true; } }
        public float            ortographicSize { get => _ortographicSize; set { _ortographicSize = value; _dirty = true; } }
        public float            nearPlane { get => _nearPlane; set { _nearPlane = value; _dirty = true; } }
        public float            farPlane { get => _farPlane; set { _farPlane = value; _dirty = true; } }
        public float            fieldOfView { get => _fieldOfView; set { _fieldOfView = value; _dirty = true; } }

        public Matrix4 projection
        {
            get
            {
                if (_dirty) UpdateInternals();

                return _projection;
            }
        }

        private void UpdateInternals()
        {
            var aspect = OpenTKApp.APP.aspectRatio;

            if (_ortographic)
            {
                _projection = Matrix4.CreateOrthographic(2.0f * _ortographicSize * aspect, 2.0f * _ortographicSize, _nearPlane, _farPlane);
            }
            else
            {
                _projection = Matrix4.CreatePerspectiveFieldOfView(_fieldOfView, aspect, _nearPlane, _farPlane);
            }

            _dirty = false;
        }
    }
}
