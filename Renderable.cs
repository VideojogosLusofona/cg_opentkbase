namespace OpenTKBase
{
    public abstract class Renderable : Component
    {
        public abstract void Render(Camera camera);
        public abstract void RenderImmediate(Camera camera);
    }
}
