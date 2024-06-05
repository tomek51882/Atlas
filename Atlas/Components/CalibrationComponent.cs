using Atlas.Core;
using Atlas.Primitives;
namespace Atlas.Components
{
    internal class CalibrationComponent : ComponentBase
    {
        private Container Parent { get; set; }
        private Container Container1 { get; set; }
        private Container Container2 { get; set; }
        public override void OnInitialized()
        {
            Parent = new Container();
            Parent.Rect = new Types.Rect();
            Parent.StyleProperties.Color = new Core.Styles.StyleProperty<Types.Color>(new Types.Color(0xed143d));
            Parent.StyleProperties.Border = new Core.Styles.StyleProperty<bool>(true);

            Parent.StyleProperties.Width = new Core.Styles.StyleProperty<Types.UnitValue<int>>(new Types.UnitValue<int>(100, Types.UnitValue<int>.Unit.Percent));
            Parent.StyleProperties.Height = new Core.Styles.StyleProperty<Types.UnitValue<int>>(new Types.UnitValue<int>(100, Types.UnitValue<int>.Unit.Percent));
            Parent.StyleProperties.Padding = new Core.Styles.StyleProperty<int>(1);



            Container1 = new Container();
            Container1.Rect = new Types.Rect();
            Container1.StyleProperties.Color = new Core.Styles.StyleProperty<Types.Color>(new Types.Color(0xed6414));
            Container1.StyleProperties.Border = new Core.Styles.StyleProperty<bool>(true);

            Container1.StyleProperties.Width = new Core.Styles.StyleProperty<Types.UnitValue<int>>(new Types.UnitValue<int>(100, Types.UnitValue<int>.Unit.Percent));
            Container1.StyleProperties.Height = new Core.Styles.StyleProperty<Types.UnitValue<int>>(new Types.UnitValue<int>(100, Types.UnitValue<int>.Unit.Percent));

            Container2 = new Container();
            Container2.Rect = new Types.Rect();
            Container2.StyleProperties.Color = new Core.Styles.StyleProperty<Types.Color>(new Types.Color(0xedaa14));
            Container2.StyleProperties.Border = new Core.Styles.StyleProperty<bool>(true);

            Container2.StyleProperties.Width = new Core.Styles.StyleProperty<Types.UnitValue<int>>(new Types.UnitValue<int>(100, Types.UnitValue<int>.Unit.Percent));
            Container2.StyleProperties.Height = new Core.Styles.StyleProperty<Types.UnitValue<int>>(new Types.UnitValue<int>(100, Types.UnitValue<int>.Unit.Percent));

            Container1.StyleProperties.Padding = new Core.Styles.StyleProperty<int>(1);
            Parent.StyleProperties.Padding = new Core.Styles.StyleProperty<int>(1);

            Container1.AddElement(Container2);
            Parent.AddElement(Container1);
        }

        public override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.AddContent(Parent);
        }
    }
}
