using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenTKBase
{
    public class Component
    {
        private GameObject _gameObject;

        public GameObject gameObject
        {
            get { return _gameObject; }
            set { _gameObject = value; }
        }

        public Transform transform => _gameObject.transform;

        public T GetComponent<T>() where T : Component => gameObject?.GetComponent<T>();

        public virtual void Update()
        {

        }
    }
}
