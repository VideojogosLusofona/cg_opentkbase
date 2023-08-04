using OpenTK.Mathematics;
using System.Collections.Generic;

namespace OpenTKBase
{
    public class Scene
    {
        private List<GameObject> objects = new List<GameObject>();

        public Material environment { get; private set; }

        public Scene()
        {
            environment = new Material(null);
            environment.SetInt("LightCount", 0);
            for (int i = 0; i < 8; i++)
            {
                environment.SetInt($"Lights[{i}].type", 0);
                environment.SetVector3($"Lights[{i}].position", Vector3.Zero);
                environment.SetVector3($"Lights[{i}].direction", Vector3.UnitZ);
                environment.SetColor($"Lights[{i}].color", Color4.White);
                environment.SetFloat($"Lights[{i}].intensity", 1.0f);
                environment.SetVector2($"Lights[{i}].spot", Vector2.Zero);
            }
        }

        public void Add(GameObject go)
        {
            objects.Add(go);            
        }

        public List<T> FindObjectsOfType<T>() where T: Component
        {
            List<T> ret = new List<T>();

            foreach (GameObject go in objects)
            {
                ret.AddRange(go.GetComponents<T>());
            }

            return ret;
        }

        public List<GameObject> GetAllObjects() => objects;
    }
}
