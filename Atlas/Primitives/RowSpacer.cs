using Atlas.Core.Styles;
using Atlas.Interfaces.Renderables;
using Atlas.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Primitives
{
    internal class RowSpacer : IPrimitive, IPrimitiveFiller
    {
        public Rect Rect { get; set; }

        public StyleProperties StyleProperties { get; set; }  = new StyleProperties();

        public RowSpacer() {
            StyleProperties.Color = new ColorProperty(Color.DefaultForeground);
        }
    }
}
