﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Interfaces.Renderables
{
    internal interface IPrimitiveProxy
    {
        IRenderable Renderable { get; }
    }
}
