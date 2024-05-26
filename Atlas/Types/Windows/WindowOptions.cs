﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Types.Windows
{
    internal struct WindowOptions
    {
        public static WindowOptions Default => new WindowOptions { Frameless = false, WindowShortcut = new ConsoleKeyInfo('\0', ConsoleKey.None, false, false, false) };
        public bool Frameless { get; set; }
        public ConsoleKeyInfo WindowShortcut { get; set; }
    }
}
