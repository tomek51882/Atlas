using Atlas.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Atlas.Components
{
    internal class Item : BaseItem
    {
        public Item() { }
        public Item(string value)
        {
            Value = value;
        }
    }
}
