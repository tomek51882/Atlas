using Atlas.Types;
using Atlas.Types.Style;

namespace Atlas.Core.Styles
{
    internal class StyleProperties
    {
        internal StyleProperty<UnitValue<int>>? Width { get; set; }
        internal StyleProperty<UnitValue<int>>? Height { get; set; }
        internal StyleProperty<FlexDirection> FlexDirection { get; set; } = new StyleProperty<FlexDirection>(Types.Style.FlexDirection.Row);
        internal StyleProperty<int>? Flex { get; set; }
        internal StyleProperty<JustifyContent>? JustifyContent { get; set; }
        internal StyleProperty<Align>? AlignItems { get; set; }
        internal StyleProperty<Align>? AlignSelf { get; set; }
        internal StyleProperty<Position> Position { get; set; } = new StyleProperty<Position>(Types.Style.Position.Relative);
        internal StyleProperty<int>? Gap { get; set; }
        internal StyleProperty<int>? ZIndex { get; set; }
        internal StyleProperty<Display>? Display { get; set; }
        internal StyleProperty<int>? Top { get; set; }
        internal StyleProperty<int>? Left { get; set; }
        internal StyleProperty<int>? Right { get; set; }
        internal StyleProperty<int>? Bottom { get; set; }
        internal StyleProperty<int>? Padding { get; set; }
        internal StyleProperty<int>? PaddingHorizontal { get; set; }
        internal StyleProperty<int>? PaddingVertical { get; set; }
        internal StyleProperty<int>? PaddingLeft { get; set; }
        internal StyleProperty<int>? PaddingRight { get; set; }
        internal StyleProperty<int>? PaddingTop { get; set; }
        internal StyleProperty<int>? PaddingBottom { get; set; }
        internal StyleProperty<Color>? Color { get; set; }
        internal StyleProperty<Color>? BackgroundColor { get; set; }
        internal StyleProperty<bool>? Border { get; set; }
        internal StyleProperty<Overflow>? Overflow { get; set; }
    }
}
