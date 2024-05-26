using Atlas.Core.Styles;
using Atlas.Interfaces.Renderables;
using Atlas.Types;

namespace Atlas.Primitives
{
    internal class Text : IPrimitive, IPrimitiveText
    {
        private string? _text;
        public Rect Rect { get; set; }
        public string? Value { get => _text;
            set
            {
                _text = value;
                Rect = new Rect(0, 0, value?.Length ?? 0, 1);
            }
        }
        public StyleProperties StyleProperties { get; set; } = new StyleProperties();

        public Text() {}

        public Text(string? value)
        {
            Value = value;
            Rect = new Rect(0,0, Value?.Length ?? 0, 1);
        }
        public Text CopyStyle(Text text)
        {
            this.StyleProperties = text.StyleProperties;
            return this;
        }
        //public void UpdateText(string? value)
        //{
        //    Value = value;
        //    Rect = new Rect(0, 0, value?.Length ?? 0, 1);
        //}
    }
}
