using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Interfaces.Renderables
{
    //internal interface IWindowable<T> : IWindowable where T : IComponent
    //{
    //    T? Component { get; }
    //}
    internal interface IWindowable : IPrimitive
    {
        string WindowId { get; }
        IComponent? Component { get; }
        bool IsFocused { get; }
        string? Title { get; }
    }
}
