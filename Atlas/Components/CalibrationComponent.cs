using Atlas.Core;
using Atlas.Primitives;
namespace Atlas.Components
{
    internal class CalibrationComponent : ComponentBase
    {
        private Container Container1 { get; set; }
        private Container Container2 { get; set; }
        public override void OnInitialized()
        {
            Container1 = new Container();
            Container1.Rect = new Types.Rect();
            Container1.StyleProperties.Color = new Core.Styles.StyleProperty<Types.Color>(new Types.Color(0x0000ff));
            Container1.StyleProperties.Border = new Core.Styles.StyleProperty<bool>(true);

            Container1.StyleProperties.Width = new Core.Styles.StyleProperty<Types.UnitValue<int>>(new Types.UnitValue<int>(100, Types.UnitValue<int>.Unit.Percent));
            Container1.StyleProperties.Height = new Core.Styles.StyleProperty<Types.UnitValue<int>>(new Types.UnitValue<int>(100, Types.UnitValue<int>.Unit.Percent));

            Container2 = new Container();
            Container2.Rect = new Types.Rect();
            Container2.StyleProperties.Color = new Core.Styles.StyleProperty<Types.Color>(new Types.Color(0xff00ff));
            Container2.StyleProperties.Border = new Core.Styles.StyleProperty<bool>(true);

            Container2.StyleProperties.Width = new Core.Styles.StyleProperty<Types.UnitValue<int>>(new Types.UnitValue<int>(100, Types.UnitValue<int>.Unit.Percent));
            Container2.StyleProperties.Height = new Core.Styles.StyleProperty<Types.UnitValue<int>>(new Types.UnitValue<int>(100, Types.UnitValue<int>.Unit.Percent));
        }

        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(Container1);
        }
    }
}
