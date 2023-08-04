using OpenTK.Mathematics;
using OpenTKBase;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace SDLBase
{
    public static class OpenTKProgram
    {
        public static void Main()
        {
            OpenTKApp app = new OpenTKApp(1280, 720, "Forest", true);

            app.Initialize();
            app.LockMouse(true);

            ExecuteApp_Forest(app);
            //ExecuteApp_NoTransform(app);

            app.Shutdown();
        }

        static GameObject CreateGround(float size)
        {
            Mesh mesh = GeometryFactory.AddPlane(size, size);

            Material material = new Material(Shader.Find("Shaders/phong"));
            material.SetColor("Color", Color4.DarkGreen);
            material.SetColor("ColorEmissive", Color4.Black);

            GameObject go = new GameObject();
            go.transform.position = new Vector3(0, 0, 0);
            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.material = material;

            return go;
        }

        static float Range(this Random rnd, float a, float b)
        {
            return rnd.NextSingle() * (b - a) + a;
        }

        static void CreateRandomTree(Random rnd, float forestSize)
        {
            float s = forestSize * 0.5f;

            // Trunk
            float heightTrunk = rnd.Range(0.5f, 1.5f);
            float widthTrunk = rnd.Range(0.7f, 1.25f);

            Mesh mesh = GeometryFactory.AddCylinder(widthTrunk, heightTrunk, 8);

            Material material = new Material(Shader.Find("Shaders/phong"));
            material.SetColor("Color", new Color4(rnd.Range(0.6f, 0.9f), rnd.Range(0.4f, 0.6f), rnd.Range(0.15f, 0.35f), 1.0f));
            material.SetColor("ColorEmissive", Color4.Black);

            GameObject mainObject = new GameObject();
            mainObject.transform.position = new Vector3(rnd.Range(-s, s), 0, rnd.Range(-s, s));
            MeshFilter mf = mainObject.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            MeshRenderer mr = mainObject.AddComponent<MeshRenderer>();
            mr.material = material;

            // Leaves
            mesh = GeometryFactory.AddCylinder(rnd.Range(widthTrunk * 1.5f, widthTrunk * 4.0f), rnd.Range(heightTrunk * 2.0f, heightTrunk * 8.0f));

            material = new Material(Shader.Find("Shaders/phong"));
            material.SetColor("Color", new Color4(rnd.Range(0.0f, 0.2f), rnd.Range(0.6f, 0.8f), rnd.Range(0.0f, 0.2f), 1.0f));
            material.SetColor("ColorEmissive", Color4.Black);

            GameObject leaveObj = new GameObject();
            leaveObj.transform.position = mainObject.transform.position + Vector3.UnitY * heightTrunk;
            leaveObj.transform.SetParent(mainObject.transform);
            mf = leaveObj.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            mr = leaveObj.AddComponent<MeshRenderer>();
            mr.material = material;
        }

        static void SetupEnvironment()
        {
            var env = OpenTKApp.APP.mainScene.environment;

            env.SetColor("Color", Color.Black);// new Color4(0.2f, 0.2f, 0.2f, 1.0f));
            env.SetColor("ColorTop", new Color4(0.0f, 1.0f, 1.0f, 1.0f));
            env.SetColor("ColorMid", new Color4(1.0f, 1.0f, 1.0f, 1.0f));
            env.SetColor("ColorBottom", new Color4(0.0f, 0.25f, 0.0f, 1.0f));
        }

        static void SetupLights()
        {
            // Setup directional light turned 30 degrees down
            GameObject go = new GameObject();
            go.transform.position = new Vector3(0.0f, 2.0f, 0.0f);
            go.transform.rotation = Quaternion.FromAxisAngle(Vector3.UnitX, -MathF.PI * 0.16f);
            Light light = go.AddComponent<Light>();
            light.type = Light.Type.Spot;
            light.lightColor = Color.White;
            light.intensity = 1.0f;
            light.cone = new Vector2(0.0f, MathF.PI / 4.0f);
        }

        static (GameObject, Material) CreateSphere()
        {
            Mesh mesh = GeometryFactory.AddSphere(1.0f, 32);

            Material material = new Material(Shader.Find("Shaders/phong"));
            material.SetColor("Color", Color4.White);
            material.SetColor("ColorEmissive", Color4.Black);

            GameObject go = new GameObject();
            go.transform.position = new Vector3(0, 2, -5);
            MeshFilter mf = go.AddComponent<MeshFilter>();
            mf.mesh = mesh;
            MeshRenderer mr = go.AddComponent<MeshRenderer>();
            mr.material = material;

            return (go, material);
        }

        static GameObject CreateForest()
        {
            float forestSize = 120.0f;

            // Create ground
            var ret = CreateGround(forestSize);

            // Create a sphere in the middle of the forest
            var (reflectSphere, reflectMaterial) = CreateSphere();
            var (glowSphere, glowMaterial) = CreateSphere();
            glowSphere.transform.position += Vector3.UnitX * 4.0f - Vector3.UnitZ * 2.0f;
            glowMaterial.SetColor("Color", Color4.Black);
            glowMaterial.SetColor("ColorEmissive", Color4.Yellow);

            // Create trees
            Random rnd = new Random(1);
            for (int i = 0; i < 50; i++)
            {
                CreateRandomTree(rnd, forestSize);
            }

            return ret;
        }

        static void ExecuteApp_Forest(OpenTKApp app)
        {
            SetupEnvironment();

            SetupLights();

            var ground = CreateForest();

            // Create camera
            GameObject cameraObject = new GameObject();
            Camera camera = cameraObject.AddComponent<Camera>();
            camera.transform.position = new Vector3(0.0f, 2.0f, 0.0f);
            camera.ortographic = false;
            FirstPersonController fps = cameraObject.AddComponent<FirstPersonController>();

            // Create pipeline
            RPS renderPipeline = new RPS();

            app.Run(() =>
            {
                app.Render(renderPipeline);
            });
        }
    }
}
