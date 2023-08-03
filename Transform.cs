using OpenTK.Mathematics;

namespace OpenTKBase
{
    public class Transform : Component
    {
        private bool        dirty;
        private Vector3     _localPosition;
        private Quaternion  _localRotation;
        private Vector3     _localScale;
        private Vector3     _worldPosition;
        private Quaternion  _worldRotation;
        private Vector3     _worldScale;
        private Vector3     _worldForward;
        private Vector3     _worldRight;
        private Vector3     _worldUp;
        private Transform   _parent;
        private Matrix4     _localMatrix;
        private Matrix4     _localToWorldMatrix;
        private Matrix4     _worldToLocalMatrix;

        public Vector3      localPosition
        {
            get { return _localPosition; }
            set { _localPosition = value; dirty = true; }
        }
        public Quaternion localRotation
        {
            get { return _localRotation; }
            set { _localRotation = value; dirty = true; }
        }
        public Vector3 localScale
        {
            get { return _localScale; }
            set { _localScale = value; dirty = true; }
        }

        public Vector3 position
        {
            get { if (dirty) UpdateInternals(); return _worldPosition; }
            set { if (_parent == null) _localPosition = value; else _localPosition = (new Vector4(value, 1.0f) * _parent.worldToLocalMatrix).Xyz; dirty = true; }
        }

        public Quaternion rotation
        {
            get { if (dirty) UpdateInternals(); return _worldRotation; }
            set { if (_parent == null) _localRotation = value; else _localRotation = (Matrix4.CreateFromQuaternion(value) * _parent.worldToLocalMatrix).ExtractRotation(true); dirty = true; }
        }

        public Vector3 lossyScale
        {
            get { if (dirty) UpdateInternals(); return _worldScale; }
            set { if (_parent == null) _localScale = value; else _localScale = (new Vector4(value, 1.0f) * _parent.worldToLocalMatrix).Xyz; dirty = true; }
        }

        public Vector3 forward
        {
            get { if (dirty) UpdateInternals(); return _worldForward; }
        }
        public Vector3 right
        {
            get { if (dirty) UpdateInternals(); return _worldRight; }
        }
        public Vector3 up
        {
            get { if (dirty) UpdateInternals(); return _worldUp; }
        }

        public Matrix4      worldToLocalMatrix
        {
            get { if (dirty) UpdateInternals(); return _worldToLocalMatrix; }
        }
        public Matrix4      localToWorldMatrix
        {
            get { if (dirty) UpdateInternals(); return _localToWorldMatrix; }
        }

        public Transform    parent
        {
            get { return _parent; }
            set { SetParent(value); }
        }

        public Transform()
        {
            localPosition = Vector3.Zero;
            localRotation = Quaternion.Identity;
            localScale = Vector3.One;
            dirty = true;
        }

        public void SetParent(Transform parent)
        {
            if (_parent == parent) return;

            Vector3     worldPosition = position;
            Quaternion  worldRotation = rotation;
            Vector3     worldScale = lossyScale;

            _parent = parent;

            if (_parent != null)
            {
                Matrix4 parentInvMatrix = _parent.worldToLocalMatrix;

                _localPosition = (new Vector4(worldPosition, 1.0f) * parentInvMatrix).Xyz;
                _localRotation = (Matrix4.CreateFromQuaternion(worldRotation) * parentInvMatrix).ExtractRotation(true);
                _localScale = (Matrix4.CreateScale(worldScale) * parentInvMatrix).ExtractScale();
            }
            else
            {
                _localPosition = worldPosition;
                _localRotation = worldRotation;
                _localScale = worldScale;
            }

            dirty = true;
        }

        private void UpdateInternals()
        {
            _localMatrix = Matrix4.CreateScale(_localScale) * Matrix4.CreateFromQuaternion(_localRotation) * Matrix4.CreateTranslation(_localPosition);
            if (_parent != null)
            {
                _localToWorldMatrix = _localMatrix * _parent.localToWorldMatrix;
            }
            else
            {
                _localToWorldMatrix = _localMatrix;
            }
            _worldToLocalMatrix = _localToWorldMatrix.Inverted();

            _worldPosition = (new Vector4(0.0f, 0.0f, 0.0f, 1.0f) * _localToWorldMatrix).Xyz;
            _worldRotation = _localToWorldMatrix.ExtractRotation(true);
            _worldScale = (new Vector4(1.0f, 1.0f, 1.0f, 0.0f) * _localToWorldMatrix).Xyz;
            _worldForward = (-Vector4.UnitZ * _localToWorldMatrix).Xyz;
            _worldRight = (Vector4.UnitX * _localToWorldMatrix).Xyz;
            _worldUp = (Vector4.UnitY * _localToWorldMatrix).Xyz;

            dirty = false;
        }
    }
}
