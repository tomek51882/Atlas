using Atlas.Types;
namespace Atlas.Core.Styles
{
    internal class ColorProperty
    {
        internal Color Value { get; set; }

        public ColorProperty(Color color)
        {
            Value = color;
        }
        public ColorProperty(int hexValue)
        {
            Value = new Color(hexValue);
        }
        public ColorProperty(byte r, byte g, byte b)
        {
            Value = new Color(r,g,b);
        }
    }
}
