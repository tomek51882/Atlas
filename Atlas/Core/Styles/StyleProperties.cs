
using Atlas.Interfaces.Styles;
using Atlas.Types;

namespace Atlas.Core.Styles
{
    internal class StyleProperties
    {
        internal StyleProperty<int>? Padding {  get; set; }
        internal StyleProperty<Color>? Color { get; set; }
        internal StyleProperty<Color>? BackgroundColor { get; set; }
        internal StyleProperty<int>? ZIndex { get; set; }
        internal StyleProperty<UnitValue<int>>? Width { get; set; }
        internal StyleProperty<UnitValue<int>>? Height { get; set; }
    }
}
