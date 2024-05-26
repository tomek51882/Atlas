using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Types
{
    internal struct UnitValue<T>
    {
        internal T value;
        internal Unit unit;

        internal UnitValue(T value, Unit unit)
        {
            this.value = value;
            this.unit = unit;
        }

        internal enum Unit
        {
            Char,
            Percent
        }
    }
}
