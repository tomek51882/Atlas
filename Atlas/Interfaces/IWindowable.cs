using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Interfaces
{
    internal interface IWindowable : IPrimitive
    {
        bool IsFocused { get; }
        string? Title { get; }
    }
}
