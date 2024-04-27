﻿using Atlas.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Interfaces
{
    internal interface IPrimitive : IRenderable
    {
        Rect Rect { get; }
    }
}
