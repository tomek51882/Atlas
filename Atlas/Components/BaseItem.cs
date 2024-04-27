using Atlas.Core;

namespace Atlas.Components
{
    internal class BaseItem : IBaseItem
    {
        public string Value { get; set; }

        public virtual void BuildRenderTree(RenderTreeBuilder builder)
        {
            
        }
    }
}
