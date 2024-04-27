using Atlas.Interfaces;
using Atlas.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Primitives
{
    internal class Container : IPrimitive, IPrimitiveContainer
    {
        public Rect Rect { get; set; }
        public List<IRenderable> Children {  get; private set; } = new List<IRenderable>();

        public Container()
        {
            
        }
    }
}
