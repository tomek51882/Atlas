

using Atlas.Core.Styles;
using Atlas.Core;
using Atlas.Primitives;
using Atlas.Types;

namespace Atlas.Components
{
    internal class StatusBar : ComponentBase
    {
        public Text text;
        private RowLayout layout;

        public override void OnInitialized()
        {
            layout = new RowLayout();

            text = new Text("Status Bar");
            text.StyleProperties.BackgroundColor = new StyleProperty<Color>(new Color(0x232428));

            var spacer = new RowSpacer();
            spacer.StyleProperties.BackgroundColor = new StyleProperty<Color>(new Color(0x232428));

            layout.Add(text);
            layout.Add(spacer);
            layout.Add(new Text("[D] Diagnostics").CopyStyle(text));

            layout.StyleProperties.Width = new StyleProperty<UnitValue<int>>(new UnitValue<int>(100, UnitValue<int>.Unit.Percent));
            layout.StyleProperties.Height = new StyleProperty<UnitValue<int>>(new UnitValue<int>(1, UnitValue<int>.Unit.Char));
        }

        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(layout);
        }
    }
}
