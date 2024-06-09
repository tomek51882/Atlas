using Atlas.Core;
using Atlas.Core.Styles;
using Atlas.Primitives;
using Atlas.Types;
using Atlas.Types.Style;
namespace Atlas.Components
{
    internal class CalibrationComponent : ComponentBase
    {
        private Container Parent { get; set; }
        private Container Container1 { get; set; }
        private Container Container2 { get; set; }
        private Container Container3 { get; set; }
        public override void OnInitialized()
        {
            Container1 = new Container();
            Container1.Rect = new Rect();
            Container1.StyleProperties.Color = new StyleProperty<Color>(new Color(0xa7007d));
            Container1.StyleProperties.Border = new StyleProperty<bool>(true);

            Container1.StyleProperties.Width = new StyleProperty<UnitValue<int>>(new UnitValue<int>(100, Unit.Percent));
            Container1.StyleProperties.Height = new StyleProperty<UnitValue<int>>(new UnitValue<int>(5, Unit.Char));

            Container2 = new Container();
            Container2.Rect = new Rect();
            Container2.StyleProperties.Color = new StyleProperty<Color>(new Color(0x6d00a7));
            Container2.StyleProperties.Border = new StyleProperty<bool>(true);
                     
            Container2.StyleProperties.Width = new StyleProperty<UnitValue<int>>(new UnitValue<int>(4, Unit.Char));
            Container2.StyleProperties.Height = new StyleProperty<UnitValue<int>>(new UnitValue<int>(5, Unit.Char));

            Container3 = new Container();
            Container3.Rect = new Rect();
            Container3.StyleProperties.Color = new StyleProperty<Color>(new Color(0x3700a7));
            Container3.StyleProperties.Border = new StyleProperty<bool>(true);
                     
            Container3.StyleProperties.Width = new StyleProperty<UnitValue<int>>(new UnitValue<int>(100, Unit.Percent));
            Container3.StyleProperties.Height = new StyleProperty<UnitValue<int>>(new UnitValue<int>(5, Unit.Char));
        }

        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(Container1);
            builder.AddContent(Container2);
            builder.AddContent(Container3);
        }
    }
}
