using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Core.Styles
{
    internal class PaddingProperty
    {
        internal int Value { get; set; }

        internal PaddingProperty(int value)
        {
            Value = value;
        }
    }
}
