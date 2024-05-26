using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Core.Styles
{
    internal class StyleProperty<T>
    {
        public T Value { get; set; }
        public StyleProperty(T value)
        {
            Value = value;
        }
    }
}
