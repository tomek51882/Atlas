using Atlas.Core.Styles;
using Atlas.Interfaces.Renderables;
using Atlas.Types;

namespace Atlas.Primitives
{
    internal class Container : IPrimitive, IRenderableContainer
    {
        public Rect Rect { get; set; }
        public List<IRenderable> Children {  get; set; } = new List<IRenderable>();
        public StyleProperties StyleProperties { get; set; } = new StyleProperties();

        public void AddElement(IRenderable child)
        {
            Children.Add(child);
        }

        public void RemoveElement(IRenderable child)
        {
            Children.Remove(child);
        }
    }
}
