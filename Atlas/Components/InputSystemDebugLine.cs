using Atlas.Core;
using Atlas.Core.Styles;
using Atlas.Primitives;
using Atlas.Types;


namespace Atlas.Components
{
    internal class InputSystemDebugLine : ComponentBase
    {
        public Text text;
        private RowLayout layout;

        public override void OnInitialized()
        {
            layout = new RowLayout();
            
            text = new Text("Input Bar");
            text.StyleProperties.BackgroundColor = new StyleProperty<Color>(new Color(0x2B2D31));
            layout.Add(text);
            var test = new RowSpacer();
            test.StyleProperties.BackgroundColor = new StyleProperty<Color>(new Color(0x2B2D31));
            layout.Add(test);
            layout.StyleProperties.Width = new StyleProperty<UnitValue<int>>(new UnitValue<int>(100, UnitValue<int>.Unit.Percent));
            layout.StyleProperties.Height = new StyleProperty<UnitValue<int>>(new UnitValue<int>(1, UnitValue<int>.Unit.Char));
            //layout.Rect = new Types.Rect(0, 0, 20, 1);

        }

        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(layout);
        }
    }
}
