using Atlas.Core;
using Atlas.Core.Styles;
using Atlas.Primitives;


namespace Atlas.Components
{
    internal class InputSystemDebugLine : ComponentBase
    {
        public Text text;
        private RowLayout layout;

        public override void OnInitialized()
        {
            layout = new RowLayout();
            
            text = new Text("123");
            text.StyleProperties.BackgroundColor = new StyleProperty<Types.Color>(Types.Color.Red);
            layout.Add(text);
            var test = new RowSpacer();
            test.StyleProperties.BackgroundColor = new StyleProperty<Types.Color>(Types.Color.Red);
            layout.Add(test);
            layout.Rect = new Types.Rect(0, 0, 20, 1);

        }

        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(layout);
        }
    }
}
